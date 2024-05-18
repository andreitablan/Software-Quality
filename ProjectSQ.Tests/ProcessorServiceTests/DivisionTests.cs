using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class DivisionTests
    {
        private readonly ProcessorService _processorService;

        public DivisionTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void Division_WithBothRegister_SetsRegisterValue()
        {
            // Arrange
            const ushort valueRegisterOne = 10;
            const ushort valueRegisterTwo = 5;
            const string registerOne = "reg1";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerOne] = valueRegisterOne;
            Processor.registerDictionary[registerTwo] = valueRegisterTwo;

            // Act
            var result = _processorService.Division(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(valueRegisterOne / valueRegisterTwo);
            Processor.registerDictionary[registerTwo].Should().Be(valueRegisterTwo);
        }

        [Fact]
        public void Division_WithRegisterAndConstant_SetsRegisterValue()
        {
            // Arrange
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string constantValue = "5";

            // Act
            var result = _processorService.Division(registerOne, constantValue);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(2);
        }

        [Fact]
        public void Division_WithRegisterAndMemoryLocatedWithConstantIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 10;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string operandOne = "reg1";
            Processor.registerDictionary[operandOne] = 54033;
            const string operandTwo = "mem[10]";

            // Act
            var result = _processorService.Division(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[operandOne].Should().Be(5403);
        }

        [Fact]
        public void Division_WithRegisterAndMemoryLocatedWithRegisterIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 10;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string operandOne = "reg2";
            Processor.registerDictionary[operandOne] = 54033;
            Processor.registerDictionary["reg1"] = 10;
            const string operandTwo = "mem[reg1]";

            // Act
            var result = _processorService.Division(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[operandOne].Should().Be(5403);
        }
        [Fact]
        public void Division_WithConstantValueForTheFirstOperand_ShouldFail()
        {
            // Arrange
            const string registerOne = "2";
            const string registerTwo = "reg1";

            // Act
            var result = _processorService.Division(registerOne, registerTwo);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Division_WithZeroValueForTheSecondOperand_ShouldFail()
        {
            // Arrange
            const string registerOne = "reg1";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerTwo] = 0;

            // Act
            var result = _processorService.Division(registerOne, registerTwo);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Division_WithMemoryLocatedWithConstantIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandOne = 54033;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandOne);

            const string operandOne = "mem[10]";
            const string operandTwo = "reg1";
            Processor.registerDictionary[operandTwo] = 10;

            const ushort resultOperation = 5403;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            // Act
            var result = _processorService.Division(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Division_WithMemoryLocatedWithRegisterIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandOne = 54033;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandOne);

            const string operandOne = "mem[reg1]";
            const string operandTwo = "reg2";
            Processor.registerDictionary["reg1"] = 10;
            Processor.registerDictionary["reg2"] = 10;

            const ushort resultOperation = 5403;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            // Act
            var result = _processorService.Division(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }
        [Fact]
        public void Multiplication_WithMemoryLocatedWithConstantIndexAndConstantOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, 50000);

            const ushort resultOperation = 25000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string operandOne = "mem[10]";
            const string operandTwo = "2";

            // Act
            var result = _processorService.Division(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Division_WithMemoryLocatedWithConstantIndexAndMemoryLocatedWithConstantIndex_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemoryOne = 10;
            const ushort valueOperandOne = 54033;
            MemoryActions.WriteValueToMemory(indexMemoryOne, valueOperandOne);

            const ushort indexMemoryTwo = 20;
            const ushort valueOperandTwo = 10;
            MemoryActions.WriteValueToMemory(indexMemoryTwo, valueOperandTwo);

            const string operandOne = "mem[10]";
            const string operandTwo = "mem[20]";

            const ushort resultOperation = 5403;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            // Act
            var result = _processorService.Division(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemoryOne].Should().Be(lowByte);
            Memory.programData[indexMemoryOne + 1].Should().Be(highByte);
        }
        [Fact]
        public void Division_WithInvalidOperands_ReturnsFalse()
        {
            // Arrange

            // Act
            var result = _processorService.Division("reg9", "reg1");

            // Assert
            result.Should().BeFalse();
        }
    }
}
