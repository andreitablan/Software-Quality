using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class JumpTests
    {
        private readonly ProcessorService _processorService;

        public JumpTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
        }

        [Fact]
        public void Jump_WithValidLabel_ShouldSetCurrentInstruction()
        {
            // Arrange
            const string label = "target";
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 1; // Index of "mov" instruction

            // Act
            var result = _processorService.Jump(label);

            // Assert
            result.Should().BeTrue();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void Jump_WithInvalidLabel_ShouldNotSetCurrentInstruction()
        {
            // Arrange
            const string label = "invalid_label";
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 0; // Current instruction index should remain unchanged

            // Act
            var result = _processorService.Jump(label);

            // Assert
            result.Should().BeFalse();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfEqual_WithEqualFlagSet_ShouldJump()
        {
            // Arrange
            const string label = "target";
            Processor.Equal = true;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 1; // Index of "mov" instruction

            // Act
            var result = _processorService.JumpIfEqual(label);

            // Assert
            result.Should().BeTrue();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfEqual_WithEqualFlagUnset_ShouldNotJump()
        {
            // Arrange
            const string label = "target";
            Processor.Equal = false;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 0; // Current instruction index should remain unchanged

            // Act
            var result = _processorService.JumpIfEqual(label);

            // Assert
            result.Should().BeFalse();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfNotEqual_WithNotEqualFlagSet_ShouldJump()
        {
            // Arrange
            const string label = "target";
            Processor.NotEqual = true;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 1; // Index of "mov" instruction

            // Act
            var result = _processorService.JumpIfNotEqual(label);

            // Assert
            result.Should().BeTrue();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfNotEqual_WithNotEqualFlagUnset_ShouldNotJump()
        {
            // Arrange
            const string label = "target";
            Processor.NotEqual = false;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 0; // Current instruction index should remain unchanged

            // Act
            var result = _processorService.JumpIfNotEqual(label);

            // Assert
            result.Should().BeFalse();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfLessThan_WithLessThanFlagSet_ShouldJump()
        {
            // Arrange
            const string label = "target";
            Processor.Less = true;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 1; // Index of "mov" instruction

            // Act
            var result = _processorService.JumpIfLessThan(label);

            // Assert
            result.Should().BeTrue();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfLessThan_WithLessThanFlagUnset_ShouldNotJump()
        {
            // Arrange
            const string label = "target";
            Processor.Less = false;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 0; // Current instruction index should remain unchanged

            // Act
            var result = _processorService.JumpIfLessThan(label);

            // Assert
            result.Should().BeFalse();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfGreaterThan_WithGreaterThanFlagSet_ShouldJump()
        {
            // Arrange
            const string label = "target";
            Processor.Greater = true;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 1; // Index of "mov" instruction

            // Act
            var result = _processorService.JumpIfGreaterThan(label);

            // Assert
            result.Should().BeTrue();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfGreaterThan_WithGreaterThanFlagUnset_ShouldNotJump()
        {
            // Arrange
            const string label = "target";
            Processor.Greater = false;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 0; // Current instruction index should remain unchanged

            // Act
            var result = _processorService.JumpIfGreaterThan(label);

            // Assert
            result.Should().BeFalse();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfLessThanOrEqual_WithLessThanOrEqualFlagSet_ShouldJump()
        {
            // Arrange
            const string label = "target";
            Processor.LessEqual = true;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 1; // Index of "mov" instruction

            // Act
            var result = _processorService.JumpIfLessThanOrEqual(label);

            // Assert
            result.Should().BeTrue();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfLessThanOrEqual_WithLessThanOrEqualFlagUnset_ShouldNotJump()
        {
            // Arrange
            const string label = "target";
            Processor.LessEqual = false;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 0; // Current instruction index should remain unchanged

            // Act
            var result = _processorService.JumpIfLessThanOrEqual(label);

            // Assert
            result.Should().BeFalse();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfGreaterThanOrEqual_WithGreaterThanOrEqualFlagSet_ShouldJump()
        {
            // Arrange
            const string label = "target";
            Processor.GreaterEqual = true;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 1; // Index of "mov" instruction

            // Act
            var result = _processorService.JumpIfGreaterThanOrEqual(label);

            // Assert
            result.Should().BeTrue();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }

        [Fact]
        public void JumpIfGreaterThanOrEqual_WithGreaterThanOrEqualFlagUnset_ShouldNotJump()
        {
            // Arrange
            const string label = "target";
            Processor.GreaterEqual = false;
            Memory.internalMemory = new[] { "label target", "mov reg1, 5" }; // Simulate some instructions
            const ushort expectedInstructionIndex = 0; // Current instruction index should remain unchanged

            // Act
            var result = _processorService.JumpIfGreaterThanOrEqual(label);

            // Assert
            result.Should().BeFalse();
            Memory.currentInstruction.Should().Be(expectedInstructionIndex);
        }
    }
}

