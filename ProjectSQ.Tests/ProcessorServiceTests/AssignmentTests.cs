using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class AssignmentTests
    {
        private readonly IProcessorService sut;

        public AssignmentTests()
        {
            var mockMemoryService = new Mock<IMemoryService>();

            var mockHubContext = new Mock<IHubContext<RealTimeHub>>();

            sut = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
        }

        [Fact]
        public void Assignment_Should_Update_Register_Value()
        {
            // Arrange
            var operandOne = "reg1";
            var operandTwo = "123";
            Processor.InitProcessor();

            // Act
            var result = sut.Assignment(operandOne, operandTwo);

            // Assert
            result.Should().Be(true);
            Processor.registerDictionary[operandOne].Should().Be(123);
        }

        [Fact]
        public void Assignment_Should_Return_False_With_Constant_Operand()
        {
            // Arrange
            var operandOne = "123";
            var operandTwo = "456";
            Processor.InitProcessor();

            // Act
            var result = sut.Assignment(operandOne, operandTwo);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Assignment_Should_Write_Value_To_Memory_Location()
        {
            // Arrange
            var operandOne = "mem[100]";
            var operandTwo = "789";
            Processor.InitProcessor();
            Memory.InitMemory();

            // Act
            var result = sut.Assignment(operandOne, operandTwo);

            // Assert
            Assert.True(result);
            var indexOperandOne = 100;

            (Memory.programData[indexOperandOne] + (Memory.programData[indexOperandOne + 1] << 8)).Should().Be(789);
        }

        [Fact]
        public void Assignment_Should_Return_False_For_Location_Beyond_Keyboard_Buffer()
        {
            // Arrange
            var operandOne = "mem[60001]";
            var operandTwo = "999";
            Processor.InitProcessor();
            Memory.InitMemory();

            // Act
            var result = sut.Assignment(operandOne, operandTwo);

            result.Should().Be(false);
        }
    }
}
