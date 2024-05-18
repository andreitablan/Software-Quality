using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class ComparisonTests : IDisposable
    {
        private readonly ProcessorService _processorService;

        public ComparisonTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        public void Dispose()
        {
            _processorService.ResetData();
        }

        [Fact]
        public void Compare_SetsZeroFlag_WhenOperandsAreEqual()
        {
            // Arrange
            const ushort value1 = 100;
            const ushort value2 = 100;

            // Act
            _processorService.Compare(value1, value2);

            // Assert
            _processorService.ZeroFlag.Should().BeTrue();
        }

        [Fact]
        public void Compare_SetsCarryFlag_WhenFirstOperandIsGreater()
        {
            // Arrange
            const ushort value1 = 200;
            const ushort value2 = 100;

            // Act
            _processorService.Compare(value1, value2);

            // Assert
            _processorService.CarryFlag.Should().BeTrue();
        }

        [Fact]
        public void Compare_SetsCarryFlag_WhenFirstOperandIsLess()
        {
            // Arrange
            const ushort value1 = 50;
            const ushort value2 = 100;

            // Act
            _processorService.Compare(value1, value2);

            // Assert
            _processorService.CarryFlag.Should().BeFalse();
        }

        [Fact]
        public void Compare_SetsNegativeFlag_WhenFirstOperandIsLess()
        {
            // Arrange
            const ushort value1 = 50;
            const ushort value2 = 100;

            // Act
            _processorService.Compare(value1, value2);

            // Assert
            _processorService.NegativeFlag.Should().BeTrue();
        }

        [Fact]
        public void Compare_SetsNegativeFlag_WhenFirstOperandIsNotLess()
        {
            // Arrange
            const ushort value1 = 150;
            const ushort value2 = 100;

            // Act
            _processorService.Compare(value1, value2);

            // Assert
            _processorService.NegativeFlag.Should().BeFalse();
        }
    }
}
