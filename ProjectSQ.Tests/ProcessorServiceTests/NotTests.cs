using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;
using ProjectSQ.Services;
using ProjectSQ.Tests.util;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class NotTests
    {
        private readonly ProcessorService _processorService;

        public NotTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void Not_WithRegister_SetsRegisterValue()
        {
            // Arrange
            const ushort valueRegisterOne = 10;
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = valueRegisterOne;

            // Act
            var result = _processorService.Not(registerOne);

            // Assert
            result.Should().BeTrue();

            const ushort notValue = unchecked((ushort)~valueRegisterOne);
            Processor.registerDictionary[registerOne].Should().Be(notValue);
        }

        [Fact]
        public void Not_WithConstantValueForTheFirstOperand_ShouldFail()
        {
            // Arrange
            const string constantValue = "10";

            // Act
            var result = _processorService.Not(constantValue);

            // Assert
            result.Should().BeFalse();
        }


        [Fact]
        public void Not_WithMemoryLocatedWithConstantIndex_SetsMemoryValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 27023;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);
            var operand = "mem[10]";

            // Act
            var result = _processorService.Not(operand);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(unchecked((byte)~valueOperandTwo));
        }
        [Fact]
        public void Division_WithInvalidOperands_ReturnsFalse()
        {
            // Arrange

            // Act
            var result = _processorService.Not("reg9");

            // Assert
            result.Should().BeFalse();
        }
    }
}
