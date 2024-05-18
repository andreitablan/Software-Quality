using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;
using ProjectSQ.Tests.util;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class AdditionTests
    {
        private readonly ProcessorService _processorService;

        public AdditionTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }
        [Fact]
        public void Addition_WithBothRegister_SetsRegisterValue()
        {
            // Arrange
            const ushort valueRegisterOne = 5;
            const ushort valueRegisterTwo = 10;
            const string registerOne = "reg1";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerOne] = valueRegisterOne;
            Processor.registerDictionary[registerTwo] = valueRegisterTwo;


            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(valueRegisterOne + valueRegisterTwo);
            Processor.registerDictionary[registerTwo].Should().Be(valueRegisterTwo);
        }

        [Fact]
        public void Addition_WithRegisterAndConstant_SetsRegisterValue()
        {
            // Arrange
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string constantValue = "5";

            // Act
            var result = _processorService.Addition(registerOne, constantValue);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(15);
        }

        [Fact]
        public void Addition_WithRegisterAndMemoryLocatedWithConstantIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 54033;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string registerTwo = "mem[10]";

            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(54043);
        }

        [Fact]
        public void Addition_WithRegisterAndMemoryLocatedWithRegisterIndex_SetsRegisterValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            Processor.registerDictionary["reg2"] = indexMemory;
            const ushort valueOperandTwo = 54033;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string registerTwo = "mem[reg2]";


            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(54043);
        }
        [Fact]
        public void Addition_WithConstantValueForTheFirstOperand_ShouldFail()
        {
            // Arrange
            const string registerOne = "2";
            const string registerTwo = "reg1";

            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Addition_WithMemoryLocatedWithConstantIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, 27000);
            
            const ushort resultOperation = 54043;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;
            const string registerOne = "mem[10]";
            const string registerTwo = "reg1";
            Processor.registerDictionary[registerTwo] = 27043;

            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Addition_WithMemoryLocatedWithRegisterIndexAndRegisterOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            Processor.registerDictionary["reg1"] = indexMemory;
            MemoryActions.WriteValueToMemory(indexMemory, 27000);

            const ushort resultOperation = 54043;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;
            const string registerOne = "mem[reg1]";
            const string registerTwo = "reg2";

            Processor.registerDictionary[registerTwo] = 27043;

            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Addition_WithMemoryLocatedWithConstantIndexAndConstantOperand_SetMemoryLocation()
        {
            // Arrange
            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, 27000);
            
            const ushort resultOperation = 54043;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;
            const string registerOne = "mem[10]";
            const string registerTwo = "27043";

            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(lowByte);
            Memory.programData[indexMemory + 1].Should().Be(highByte);
        }

        [Fact]
        public void Addition_WithMemoryLocatedWithConstantIndexAndMemoryLocatedWithConstantIndex_SetMemoryLocation()
        {
            // Arrange
            const ushort indexOperandOne = 10;
            const ushort indexOperandTwo = 20;
            MemoryActions.WriteValueToMemory(indexOperandOne, 27000);
            MemoryActions.WriteValueToMemory(indexOperandTwo, 27043);
            const ushort resultOperation = 54043;
            const byte highByte = resultOperation >> 8;
            const byte lowByte = resultOperation & 0xFF;

            const string registerOne = "mem[10]";
            const string registerTwo = "mem[20]";

            // Act
            var result = _processorService.Addition(registerOne, registerTwo);

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
            var result = _processorService.Addition("reg9", "reg1");

            // Assert
            result.Should().BeFalse();
        }
    }
}