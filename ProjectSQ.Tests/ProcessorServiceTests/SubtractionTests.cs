using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class SubtractionTests
    {
        private readonly ProcessorService _processorService;

        public SubtractionTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void Subtraction_WithBothRegister_SetsRegisterValue()
        {
            // Arrange
            const ushort valueRegisterOne = 10;
            const ushort valueRegisterTwo = 5;
            const string registerOne = "reg1";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerOne] = valueRegisterOne;
            Processor.registerDictionary[registerTwo] = valueRegisterTwo;

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(valueRegisterOne - valueRegisterTwo);
            Processor.registerDictionary[registerTwo].Should().Be(valueRegisterTwo);
        }

        [Fact]
        public void Subtraction_WithRegisterAndConstant_SetsRegisterValue()
        {
            // Arrange
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string constantValue = "5";

            // Act
            var result = _processorService.Subtraction(registerOne, constantValue);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(5);
        }

        [Fact]
        public void Subtraction_WithRegisterAndMemoryLocatedWithConstantIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 27023;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 54023;
            const string registerTwo = "mem[10]";

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(27000);
        }

        [Fact]
        public void Subtraction_WithRegisterAndMemoryLocatedWithRegisterIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            Processor.registerDictionary["reg2"] = indexMemory;
            const ushort valueOperandTwo = 27023;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 54023;
            const string registerTwo = "mem[reg2]";

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(27000);
        }

        [Fact]
        public void Subtraction_WithConstantValueForTheFirstOperand_ShouldFail()
        {
            // Arrange
            const string registerOne = "2";
            const string registerTwo = "reg1";

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Subtraction_WithMemoryLocatedWithConstantIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandOne = 54023;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandOne);

            const ushort resultOperation = 27000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string registerOne = "mem[10]";
            const string registerTwo = "reg1";
            Processor.registerDictionary[registerTwo] = 27023;

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Subtraction_WithMemoryLocatedWithRegisterIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            Processor.registerDictionary["reg1"] = indexMemory;
            const ushort valueOperandOne = 54023;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandOne);

            const ushort resultOperation = 27000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string registerOne = "mem[reg1]";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerTwo] = 27023;

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Subtraction_WithMemoryLocatedWithConstantIndexAndConstantOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexOperandOne = 10;
            MemoryActions.WriteValueToMemory(indexOperandOne, 54043);
            const string registerOne = "mem[10]";
            const string registerTwo = "27043";

            const ushort resultOperation = 27000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexOperandOne].Should().Be(lowByte);
            Memory.programData[indexOperandOne + 1].Should().Be(highByte);
        }

        [Fact]
        public void Subtraction_WithMemoryLocatedWithConstantIndexAndMemoryLocatedWithConstantIndex_SetMemoryLocation()
        {
            // Arrange
            const ushort indexOperandOne = 10;
            const ushort indexOperandTwo = 20;
            MemoryActions.WriteValueToMemory(indexOperandOne, 54043);
            MemoryActions.WriteValueToMemory(indexOperandTwo, 27043);
            const ushort resultOperation = 27000;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string registerOne = "mem[10]";
            const string registerTwo = "mem[20]";

            // Act
            var result = _processorService.Subtraction(registerOne, registerTwo);

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
            var result = _processorService.Subtraction("reg9", "reg1");

            // Assert
            result.Should().BeFalse();
        }
    }
}
