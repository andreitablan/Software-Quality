using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class JumpTests : IDisposable
    {
        private readonly ProcessorService _processorService;

        public JumpTests()
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
        public void UnconditionalJump_SetsProgramCounter()
        {
            // Arrange
            const ushort targetAddress = 100;

            // Act
            var result = _processorService.UnconditionalJump(targetAddress);

            // Assert
            result.Should().BeTrue();
            _processorService.ProgramCounter.Should().Be(targetAddress);
        }

        [Fact]
        public void ConditionalJump_WhenZeroFlagSet_JumpsToTargetAddress()
        {
            // Arrange
            const ushort targetAddress = 200;
            _processorService.SetZeroFlag(true);

            // Act
            var result = _processorService.ConditionalJump(targetAddress, Condition.Zero);

            // Assert
            result.Should().BeTrue();
            _processorService.ProgramCounter.Should().Be(targetAddress);
        }

        [Fact]
        public void ConditionalJump_WhenZeroFlagNotSet_DoesNotJump()
        {
            // Arrange
            const ushort targetAddress = 300;
            _processorService.SetZeroFlag(false);

            // Act
            var result = _processorService.ConditionalJump(targetAddress, Condition.Zero);

            // Assert
            result.Should().BeFalse();
            _processorService.ProgramCounter.Should().NotBe(targetAddress);
        }

        [Fact]
        public void ConditionalJump_WhenCarryFlagSet_JumpsToTargetAddress()
        {
            // Arrange
            const ushort targetAddress = 400;
            _processorService.SetCarryFlag(true);

            // Act
            var result = _processorService.ConditionalJump(targetAddress, Condition.Carry);

            // Assert
            result.Should().BeTrue();
            _processorService.ProgramCounter.Should().Be(targetAddress);
        }

        [Fact]
        public void ConditionalJump_WhenCarryFlagNotSet_DoesNotJump()
        {
            // Arrange
            const ushort targetAddress = 500;
            _processorService.SetCarryFlag(false);

            // Act
            var result = _processorService.ConditionalJump(targetAddress, Condition.Carry);

            // Assert
            result.Should().BeFalse();
            _processorService.ProgramCounter.Should().NotBe(targetAddress);
        }
    }
}
