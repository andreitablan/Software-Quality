using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class MultiplicationTests
    {
        private readonly ProcessorService _processorService;

        public MultiplicationTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void Multiplication_WithBothRegister_SetsRegisterValue()
        {
            // Arrange
            const ushort valueRegisterOne = 10;
            const ushort valueRegisterTwo = 5;
            const string registerOne = "reg1";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerOne] = valueRegisterOne;
            Processor.registerDictionary[registerTwo] = valueRegisterTwo;

            // Act
            var result = _processorService.Multiplication(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(valueRegisterOne * valueRegisterTwo);
            Processor.registerDictionary[registerTwo].Should().Be(valueRegisterTwo);
        }

        [Fact]
        public void Multiplication_WithRegisterAndConstant_SetsRegisterValue()
        {
            // Arrange
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string constantValue = "5";

            // Act
            var result = _processorService.Multiplication(registerOne, constantValue);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(50);
        }

        [Fact]
        public void Multiplication_WithRegisterAndMemoryLocatedWithConstantIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 25000;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 2;
            const string memoryLocation = "mem[10]";

            // Act
            var result = _processorService.Multiplication(registerOne, memoryLocation);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(50000);
        }
        [Fact]
        public void Multiplication_WithRegisterAndMemoryLocatedWithRegisterIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 25000;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 2;
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerTwo] = 10;

            // Act
            var result = _processorService.Multiplication(registerOne, $"mem[{registerTwo}]");

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(50000);
        }

        [Fact]
        public void Multiplication_WithConstantValueForTheFirstOperand_ShouldFail()
        {
            // Arrange
            const string registerOne = "2";
            const string registerTwo = "reg1";

            // Act
            var result = _processorService.Multiplication(registerOne, registerTwo);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Multiplication_WithMemoryLocatedWithConstantIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 25000;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const ushort resultOperation = 50000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string operandOne = "mem[10]";
            const string operandTwo = "reg1";
            Processor.registerDictionary[operandTwo] = 2;

            // Act
            var result = _processorService.Multiplication(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Multiplication_WithMemoryLocatedWithRegisterIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 25000;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const ushort resultOperation = 50000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string operandOne = "mem[reg2]";
            Processor.registerDictionary["reg2"] = 10;

            const string operandTwo = "reg1";
            Processor.registerDictionary[operandTwo] = 2;

            // Act
            var result = _processorService.Multiplication(operandOne, operandTwo);

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
            MemoryActions.WriteValueToMemory(indexMemory, 25000);

            const ushort resultOperation = 50000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string operandOne = "mem[10]";
            const string operandTwo = "2";

            // Act
            var result = _processorService.Multiplication(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Multiplication_WithMemoryLocatedWithConstantIndexAndMemoryLocatedWithConstantIndex_SetMemoryLocation()
        {
            // Arrange
            const ushort indexOperandOne = 10;
            MemoryActions.WriteValueToMemory(indexOperandOne, 25000);
            const ushort indexOperandTwo = 20;
            MemoryActions.WriteValueToMemory(indexOperandTwo, 2);

            const ushort resultOperation = 50000;
            const byte highByte = (byte)(resultOperation >> 8);
            const byte lowByte = (byte)(resultOperation & 0xFF);

            const string operandOne = "mem[10]";
            const string operandTwo = "mem[20]";

            // Act
            var result = _processorService.Multiplication(operandOne, operandTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexOperandOne].Should().Be(lowByte);
            Memory.programData[indexOperandOne + 1].Should().Be(highByte);
        }
        [Fact]
        public void Addition_WithInvalidOperands_ReturnsFalse()
        {
            // Arrange

            // Act
            var result = _processorService.Multiplication("reg9", "reg1");

            // Assert
            result.Should().BeFalse();
        }
    }
}
