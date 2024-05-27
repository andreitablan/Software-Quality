using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class StackOperationsTests
    {
        private readonly ProcessorService _processorService;

        public StackOperationsTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        private void WriteUshortToMemory(ushort index, ushort value)
        {
            Memory.programData[index] = (byte)(value & 0xFF);
            Memory.programData[index + 1] = (byte)(value >> 8);
        }

        private ushort ReadUshortFromMemory(ushort index)
        {
            return (ushort)(Memory.programData[index] | (Memory.programData[index + 1] << 8));
        }

        [Fact]
        public void Push_RegisterValue_SavesValueToStack()
        {
            // Arrange
            const string register = "reg1";
            Processor.registerDictionary[register] = 1234;
            Processor.StackPointer = 0;

            // Act
            _processorService.Push(register);

            // Assert
            ReadUshortFromMemory(0).Should().Be(1234);
            Processor.StackPointer.Should().Be(2);
        }

        [Fact]
        public void Push_ConstantValue_SavesValueToStack()
        {
            // Arrange
            const string constantValue = "5678";
            Processor.StackPointer = 0;

            // Act
            _processorService.Push(constantValue);

            // Assert
            ReadUshortFromMemory(0).Should().Be(5678);
            Processor.StackPointer.Should().Be(2);
        }

        [Fact]
        public void Push_MemoryLocation_SavesValueToStack()
        {
            // Arrange
            const ushort indexMemory = 10;
            const string memoryLocation = "mem[10]";
            WriteUshortToMemory(indexMemory, 1357);
            Processor.StackPointer = 0;

            // Act
            _processorService.Push(memoryLocation);

            // Assert
            ReadUshortFromMemory(0).Should().Be(1357);
            Processor.StackPointer.Should().Be(2);
        }

        [Fact]
        public void Pop_RegisterValue_RestoresValueFromStack()
        {
            // Arrange
            const string register = "reg1";
            Processor.StackPointer = 2;
            WriteUshortToMemory(0, 2468);

            // Act
            _processorService.Pop(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(2468);
            Processor.StackPointer.Should().Be(0);
            Memory.programData[0].Should().Be(0);
            Memory.programData[1].Should().Be(0);
        }

        [Fact]
        public void PushAndPop_MultipleValues_StackOperationsAreCorrect()
        {
            // Arrange
            const string register1 = "reg1";
            const string register2 = "reg2";
            const string register3 = "reg3";

            Processor.registerDictionary[register1] = 123;
            Processor.registerDictionary[register2] = 456;
            Processor.registerDictionary[register3] = 789;
            Processor.StackPointer = 0;

            // Act
            _processorService.Push(register1);
            _processorService.Push(register2);
            _processorService.Push(register3);

            _processorService.Pop(register3);
            _processorService.Pop(register2);
            _processorService.Pop(register1);

            // Assert
            Processor.registerDictionary[register1].Should().Be(123);
            Processor.registerDictionary[register2].Should().Be(456);
            Processor.registerDictionary[register3].Should().Be(789);

            Processor.StackPointer.Should().Be(0);
            Memory.programData[0].Should().Be(0);
            Memory.programData[1].Should().Be(0);
            Memory.programData[2].Should().Be(0);
            Memory.programData[3].Should().Be(0);
            Memory.programData[4].Should().Be(0);
            Memory.programData[5].Should().Be(0);
        }

        [Fact]
        public void Pop_EmptyStack_DoesNotCrash()
        {
            // Arrange
            const string register = "reg1";
            Processor.StackPointer = 0;

            // Act
            _processorService.Pop(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(0);
            Processor.StackPointer.Should().Be(0);
        }
    }
}