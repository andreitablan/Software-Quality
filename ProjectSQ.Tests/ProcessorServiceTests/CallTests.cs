using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class CallTests
    {
        private readonly ProcessorService _processorService;

        public CallTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void Call_ValidFunctionName_SetsCurrentInstruction()
        {
            // Arrange
            Memory.internalMemory = new string[]
            {
                "main",
                "mov reg1, 2",
                "call func1",
                "function func1",
                "add reg1, reg2"
            };
            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 2;
            Processor.StackPointer = 0;

            // Act
            _processorService.Call("func1");

            // Assert
            Memory.currentInstruction.Should().Be(3);
            Memory.programData[0].Should().Be(2);
            Processor.StackPointer.Should().Be(2);
        }

        [Fact]
        public void Call_InvalidFunctionName_DoesNotChangeCurrentInstruction()
        {
            // Arrange
            Memory.internalMemory = new string[]
            {
                "main",
                "mov reg1, 2",
                "call func1",
                "function func2",
                "add reg1, reg2"
            };
            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 2;
            Processor.StackPointer = 0;

            // Act
            _processorService.Call("func1");

            // Assert
            Memory.currentInstruction.Should().Be(2);
            Processor.StackPointer.Should().Be(2);
        }

        [Fact]
        public void Call_EmptyMemory_DoesNotChangeCurrentInstruction()
        {
            // Arrange
            Memory.internalMemory = new string[] { };
            Memory.instructionsNumber = 0;
            Memory.currentInstruction = 0;
            Processor.StackPointer = 0;

            // Act
            _processorService.Call("func1");

            // Assert
            Memory.currentInstruction.Should().Be(0);
            Processor.StackPointer.Should().Be(2);
        }

        [Fact]
        public void Call_FunctionNameAtDifferentPosition_SetsCurrentInstruction()
        {
            // Arrange
            Memory.internalMemory = new string[]
            {
                "main",
                "mov reg1, 2",
                "call func1",
                "nop",
                "nop",
                "function func1",
                "add reg1, reg2"
            };
            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 2;
            Processor.StackPointer = 0;

            // Act
            _processorService.Call("func1");

            // Assert
            Memory.currentInstruction.Should().Be(5);
            Processor.StackPointer.Should().Be(2);
        }
    }
}
