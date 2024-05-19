using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class ReturnTests
    {
        private readonly ProcessorService _processorService;

        public ReturnTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void Return_SetsCurrentInstructionToCaller()
        {
            // Arrange
            Memory.internalMemory = new string[]
            {
                "main",
                "mov reg1, 2",
                "call func1",
                "function func1",
                "add reg1, reg2",
                "return"
            };
            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 3;
            Processor.StackPointer = 2;
            Memory.programData[0] = 2;
            Memory.programData[1] = 0;

            // Act
            _processorService.Return();

            // Assert
            Memory.currentInstruction.Should().Be(3);
            Processor.StackPointer.Should().Be(0);
            Memory.programData[0].Should().Be(0);
            Memory.programData[1].Should().Be(0);
        }

        [Fact]
        public void Return_WithoutValidCaller_DoesNotCrash()
        {
            // Arrange
            Memory.internalMemory = new string[]
            {
                "main",
                "mov reg1, 2",
                "call func1",
                "function func1",
                "add reg1, reg2",
                "return"
            };
            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 3;
            Processor.StackPointer = 2;

            // Act
            _processorService.Return();

            // Assert
            Memory.currentInstruction.Should().Be(1);
            Processor.StackPointer.Should().Be(0);
        }

        [Fact]
        public void Return_WhenCalled_SetsCurrentInstructionProperly()
        {
            // Arrange
            Memory.internalMemory = new string[]
            {
                "main",
                "call func1",
                "nop",
                "function func1",
                "nop",
                "return",
                "nop"
            };
            Memory.instructionsNumber = (ushort)Memory.internalMemory.Length;
            Memory.currentInstruction = 3;
            Processor.StackPointer = 2;
            Memory.programData[0] = 1;
            Memory.programData[1] = 0;

            // Act
            _processorService.Return();

            // Assert
            Memory.currentInstruction.Should().Be(1);
            Processor.StackPointer.Should().Be(0);
            Memory.programData[0].Should().Be(0);
            Memory.programData[1].Should().Be(0);
        }
    }
}
