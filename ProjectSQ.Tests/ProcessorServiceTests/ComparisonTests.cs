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
    public class ComparisonTests
    {
        private readonly ProcessorService _processorService;

        public ComparisonTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void Comparison_WithValidOperands_ShouldReturnTrue()
        {
            // Arrange
            const string operand1 = "1";
            const string operand2 = "2";

            // Act
            var result = _processorService.Compare(operand1, operand2);

            // Assert
            result.Should().BeTrue();

            Processor.Equal.Should().BeFalse();
            Processor.LessEqual.Should().BeTrue();
            Processor.GreaterEqual.Should().BeFalse();

            Processor.NotEqual.Should().BeTrue();
            Processor.Less.Should().BeTrue();
            Processor.Greater.Should().BeFalse();
            // Add more assertions if needed
        }

        [Fact]
        public void Comparison_WithInvalidOperands_ShouldReturnFalse()
        {
            // Arrange
            const string operand1 = "t1";
            const string operand2 = "t2";

            // Act
            var result = _processorService.Compare(operand1, operand2);

            // Assert
            result.Should().BeFalse();
            // Add more assertions if needed
        }
        [Fact]
        public void Comparison_Should_Update_Register_Value()
        {
            // Arrange
            const ushort valueRegisterOne = 10;
            const ushort valueRegisterTwo = 5;
            const string registerOne = "reg1";
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerOne] = valueRegisterOne;
            Processor.registerDictionary[registerTwo] = valueRegisterTwo;

            // Act
            var result = _processorService.Compare(registerOne, registerTwo);

            // Assert
            result.Should().Be(true);
            Processor.Equal.Should().BeFalse();
            Processor.LessEqual.Should().BeFalse();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeTrue();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeTrue();
        }
        //create a test where the first operand is a memory location and the second operand is a constant
        [Fact]
        public void Comparison_WithMemoryLocationAndConstant_ShouldReturnTrue()
        {
            // Arrange
            const ushort indexMemory = 10;
            const ushort valueOperandTwo = 25000;
            MemoryActions.WriteValueToMemory(indexMemory, valueOperandTwo);

            const string memoryLocation = "mem[10]";
            const string constantValue = "25000";

            // Act
            var result = _processorService.Compare(memoryLocation, constantValue);

            // Assert
            result.Should().BeTrue();
            Processor.Equal.Should().BeTrue();
            Processor.LessEqual.Should().BeTrue();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeFalse();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeFalse();
        }
    }
}
