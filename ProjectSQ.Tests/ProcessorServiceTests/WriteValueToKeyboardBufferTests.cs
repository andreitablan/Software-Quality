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
    public class WriteValueToKeyboardBufferTests
    {
        private readonly IProcessorService sut;

        public WriteValueToKeyboardBufferTests()
        {
            var mockMemoryService = new Mock<IMemoryService>();

            var mockHubContext = new Mock<IHubContext<RealTimeHub>>();

            sut = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
        }

        [Fact]
        public void WriteValueToKeyboardBuffer_ShouldWriteLowByteOfValue()
        {
            // Arrange
            Memory.InitMemory();
            char charToWrite = 'A';
            ushort valueToWrite = (ushort)charToWrite;

            // Act
            sut.WriteValueToKeyboardBuffer(valueToWrite);

            // Assert
            Memory.programData[Memory.keyboardBufferIndex].Should().Be((byte)'A');
        }

        [Fact]
        public void WriteValueToKeyboardBuffer_ShouldSetKeyboardBufferChangedFlag()
        {
            // Arrange
            Memory.InitMemory();
            char charToWrite = 'A';
            ushort valueToWrite = (ushort)charToWrite;

            // Act
            sut.WriteValueToKeyboardBuffer(valueToWrite);

            // Assert
            Memory.isKeyboardBufferChanged.Should().BeTrue();
        }
    }
}
