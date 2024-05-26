using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class ReadFromVideoMemoryTests
    {

        private readonly IProcessorService sut;

        public ReadFromVideoMemoryTests()
        {
            var mockMemoryService = new Mock<IMemoryService>();

            var mockHubContext = new Mock<IHubContext<RealTimeHub>>();

            sut = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
        }
        [Fact]
        public void ReadFromVideoMemory_ReturnsEmptyString_WhenVideoMemoryIsEmpty()
        {
            // Arrange
            Memory.InitMemory();
            Memory.WipeVideoMemory();

            // Act
            string result = sut.ReadFromVideoMemory();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ReadFromVideoMemory_ReturnsCorrectString_WhenVideoMemoryIsPopulated()
        {
            // Arrange
            Memory.InitMemory();
            Memory.WipeVideoMemory();
            string expected = "Hello, World!";

            for (int i = 0; i < expected.Length; i++)
            {
                Memory.programData[Memory.firstVideoMemoryIndex + i] = (byte)expected[i];
            }
            Memory.lastIndexOfMemoryVideo = (ushort)(Memory.firstVideoMemoryIndex + expected.Length);

            // Act
            string result = sut.ReadFromVideoMemory();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ReadFromVideoMemory_ReturnsPartialString_WhenVideoMemoryIsPartiallyPopulated()
        {
            // Arrange
            Memory.InitMemory();
            Memory.WipeVideoMemory();
            string expected = "Test";
            for (int i = 0; i < expected.Length; i++)
            {
                Memory.programData[Memory.firstVideoMemoryIndex + i] = (byte)expected[i];
            }
            Memory.lastIndexOfMemoryVideo = (ushort)(Memory.firstVideoMemoryIndex + expected.Length);

            // Act
            string result = sut.ReadFromVideoMemory();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
