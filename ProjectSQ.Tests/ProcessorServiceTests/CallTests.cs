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
                "function test",
                "mov reg3,15",
                "mov reg4,20",
                "add reg5,reg3",
                "add reg5,reg4",
                "return",
                "main",
                "mov reg1,10",
                "mov reg2,20",
                "call test",
                "mov reg8,reg7"
            };

            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 6;
            Processor.StackPointer = 0;

            // Act
            _processorService.Call("test");

            // Assert
            Memory.currentInstruction.Should().Be(0);
            Memory.programData[0].Should().Be(6);
            Processor.StackPointer.Should().Be(2);
        }

        //Fails as we don't treat the case with invalid function call
        [Fact]
        public void Call_InvalidFunctionName_DoesNotChangeCurrentInstruction()
        {
            // Arrange
            Memory.internalMemory = new string[]
            {
                "function test",
                "mov reg3,15",
                "mov reg4,20",
                "add reg5,reg3",
                "add reg5,reg4",
                "return",
                "main",
                "mov reg1,10",
                "mov reg2,20",
                "call test1",
                "mov reg8,reg7"
            };

            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 6;
            Processor.StackPointer = 0;

            // Act
            _processorService.Call("test1");

            // Assert
            Memory.currentInstruction.Should().Be(10);
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
                "function test",
                "mov reg3,15",
                "mov reg4,20",
                "add reg5,reg3",
                "add reg5,reg4",
                "return",
                "main",
                "mov reg1,10",
                "mov reg2,20",
                "call test",
                "mov reg8,reg7"
            };

            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 6;
            Processor.StackPointer = 0;

            // Act
            _processorService.Call("test");

            // Assert
            Memory.currentInstruction.Should().Be(0);
            Processor.StackPointer.Should().Be(2);
        }
    }
}
