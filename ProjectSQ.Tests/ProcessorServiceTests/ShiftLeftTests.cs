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
    public class ShiftLeftTests
    {
        private readonly ProcessorService _processorService;

        public ShiftLeftTests()
        {
            Mock<IHubContext<RealTimeHub>> mockHubContext = new();
            Mock<IMemoryService> mockMemoryService = new();
            _processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();
        }

        [Fact]
        public void ShiftLeft_WithRegisterAndConstant_SetsRegisterValue()
        {
            // Arrange
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string constantValue = "2";

            // Act
            var result = _processorService.ShiftLeft(registerOne, constantValue);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(40); // 10 << 2 == 40
        }

        [Fact]
        public void ShiftLeft_WithRegisterAndRegister_SetsRegisterValue()
        {
            // Arrange
            const string registerOne = "reg1";
            Processor.registerDictionary[registerOne] = 10;
            const string registerTwo = "reg2";
            Processor.registerDictionary[registerTwo] = 2;

            // Act
            var result = _processorService.ShiftLeft(registerOne, registerTwo);

            // Assert
            result.Should().BeTrue();
            Processor.registerDictionary[registerOne].Should().Be(40); // 10 << 2 == 40
        }

        [Fact]
        public void ShiftLeft_WithMemoryLocationAndConstantIndex_SetsMemoryValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            MemoryActions.WriteValueToMemory(indexMemory, 10);
            const string memoryLocation = "mem[10]";
            const string constantValue = "2";

            // Act
            var result = _processorService.ShiftLeft(memoryLocation, constantValue);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(40); // 10 << 2 == 40
        }

        [Fact]
        public void ShiftLeft_WithMemoryLocationAndRegisterIndex_SetsMemoryValue()
        {
            // Arrange
            const ushort indexMemory = 10;
            Processor.registerDictionary["reg2"] = 2;
            MemoryActions.WriteValueToMemory(indexMemory, 10);
            const string memoryLocation = "mem[10]";
            const string registerTwo = "reg2";

            // Act
            var result = _processorService.ShiftLeft(memoryLocation, registerTwo);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory].Should().Be(40); // 10 << 2 == 40
        }

        [Fact]
        public void ShiftLeft_WithInvalidConstantFirstOperand_ShouldFail()
        {
            // Arrange
            const string constantOperand = "2";
            const string registerTwo = "reg1";

            // Act
            var result = _processorService.ShiftLeft(constantOperand, registerTwo);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ShiftLeft_WithMemoryLocationAndMemoryLocation_SetsMemoryValue()
        {
            // Arrange
            const ushort indexMemory1 = 10;
            const ushort indexMemory2 = 20;
            MemoryActions.WriteValueToMemory(indexMemory1, 10);
            MemoryActions.WriteValueToMemory(indexMemory2, 2);
            const string memoryLocation1 = "mem[10]";
            const string memoryLocation2 = "mem[20]";

            // Act
            var result = _processorService.ShiftLeft(memoryLocation1, memoryLocation2);

            // Assert
            result.Should().BeTrue();
            Memory.programData[indexMemory1].Should().Be(40); // 10 << 2 == 40
        }
    }
}
