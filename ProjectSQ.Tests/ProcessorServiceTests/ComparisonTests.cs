using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;

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

        // Add more test cases as needed
    }
}
