using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class OrTests
    {
        private readonly ProcessorService _processorService;

        public OrTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void And_WithBothRegister_SetsRegisterValue()
        {
            // Arrange
            const ushort valueRegisterOne = 10;
            const ushort valueRegisterTwo = 5;
            const string registerOne = "reg1";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerOne] = valueRegisterOne;
            Processor.registerDictionary[registerTwo] = valueRegisterTwo;

            // Act
            var result = _processorService.Or(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(valueRegisterOne | valueRegisterTwo);
            Processor.registerDictionary[registerTwo].Should().Be(valueRegisterTwo);
        }

        [Fact]
        public void And_WithRegisterAndConstant_SetsRegisterValue()
        {
            // Arrange
            const ushort valueRegisterOne = 10;
            const ushort valueRegisterTwo = 5;
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string constantValue = "5";

            // Act
            var result = _processorService.Or(registerOne, constantValue);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(valueRegisterOne | valueRegisterTwo);
        }

        [Fact]
        public void And_WithRegisterAndMemoryLocatedWithConstantIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort valueOperandOne = 10;
            const ushort valueOperandTwo = 5;

            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string operandOne = "reg1";
            Processor.registerDictionary[operandOne] = valueOperandOne;
            const string operandTwo = "mem[10]";

            // Act
            var result = _processorService.Or(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[operandOne].Should().Be(valueOperandOne | valueOperandTwo);
        }

        [Fact]
        public void And_WithRegisterAndMemoryLocatedWithRegisterIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort valueOperandOne = 10;
            const ushort valueOperandTwo = 5;

            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string operandOne = "reg1";
            Processor.registerDictionary[operandOne] = valueOperandOne;
            const string operandTwo = "mem[reg1]";

            // Act
            var result = _processorService.Or(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[operandOne].Should().Be(valueOperandOne | valueOperandTwo);
        }

        [Fact]
        public void And_WithConstantValueForTheFirstOperand_ShouldFail()
        {
            // Arrange
            const string registerOne = "2";
            const string registerTwo = "reg1";

            // Act
            var result = _processorService.Or(registerOne, registerTwo);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void And_WithMemoryLocatedWithConstantIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort valueOperandOne = 10;
            const ushort valueOperandTwo = 5;
            const ushort resultOperation = valueOperandOne | valueOperandTwo;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandOne);

            const string operandOne = "mem[10]";
            const string operandTwo = "reg1";
            Processor.registerDictionary[operandTwo] = valueOperandTwo;

            // Act
            var result = _processorService.Or(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void And_WithMemoryLocatedWithRegisterIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort valueOperandOne = 10;
            const ushort valueOperandTwo = 5;
            const ushort resultOperation = valueOperandOne | valueOperandTwo;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const ushort indexMemory = 10;
            Processor.registerDictionary["reg1"] = indexMemory;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandOne);

            const string operandOne = "mem[reg1]";
            const string operandTwo = "reg2";
            Processor.registerDictionary[operandTwo] = valueOperandTwo;

            // Act
            var result = _processorService.Or(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void And_WithMemoryLocatedWithConstantIndexAndConstantOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort valueOperandOne = 10;
            const ushort valueOperandTwo = 5;
            const ushort resultOperation = valueOperandOne | valueOperandTwo;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandOne);

            const string operandOne = "mem[10]";
            const string operandTwo = "5";

            // Act
            var result = _processorService.Or(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void And_WithMemoryLocatedWithConstantIndexAndMemoryLocatedWithConstantIndex_SetMemoryLocation()
        {
            // Arrange
            const ushort valueOperandOne = 10;
            const ushort valueOperandTwo = 5;
            const ushort resultOperation = valueOperandOne | valueOperandTwo;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const ushort indexOperandOne = 10;
            MemoryActions.WriteValueToMemory(indexOperandOne, valueOperandOne);

            const ushort indexOperandTwo = 20;
            MemoryActions.WriteValueToMemory(indexOperandTwo, valueOperandTwo);

            const string operandOne = "mem[10]";
            const string operandTwo = "mem[20]";

            // Act
            var result = _processorService.Or(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexOperandOne].Should().Be(lowByte);
            Memory.programData[indexOperandOne + 1].Should().Be(highByte);
        }
        [Fact]
        public void And_WithInvalidOperands_ReturnsFalse()
        {
            // Arrange

            // Act
            var result = _processorService.Or("reg9", "reg1");

            // Assert
            result.Should().BeFalse();
        }
    }
}
