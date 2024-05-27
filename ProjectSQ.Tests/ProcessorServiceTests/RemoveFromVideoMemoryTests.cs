using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class RemoveFromVideoMemoryTests
    {
        private readonly IProcessorService sut;

        public RemoveFromVideoMemoryTests()
        {
            var mockMemoryService = new Mock<IMemoryService>();

            var mockHubContext = new Mock<IHubContext<RealTimeHub>>();

            sut = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
        }

        [Fact]
        public void RemoveFromVideoMemory_DecrementsLastIndexOfMemoryVideo()
        {
            // Arrange
            Memory.InitMemory();
            Memory.WipeVideoMemory();
            string initialData = "Hello";
            for (int i = 0; i < initialData.Length; i++)
            {
                Memory.programData[Memory.firstVideoMemoryIndex + i] = (byte)initialData[i];
            }
            Memory.lastIndexOfMemoryVideo = (ushort)(Memory.firstVideoMemoryIndex + initialData.Length);

            int expectedLastIndex = Memory.lastIndexOfMemoryVideo - 1;

            // Act
            sut.RemoveFromVideoMemory();

            // Assert
            Assert.Equal(expectedLastIndex, Memory.lastIndexOfMemoryVideo);
        }

        [Fact]
        public void RemoveFromVideoMemory_SetsLastByteToZero()
        {
            // Arrange
            Memory.InitMemory();
            Memory.WipeVideoMemory();
            string initialData = "Hello";
            for (int i = 0; i < initialData.Length; i++)
            {
                Memory.programData[Memory.firstVideoMemoryIndex + i] = (byte)initialData[i];
            }
            Memory.lastIndexOfMemoryVideo = (ushort)(Memory.firstVideoMemoryIndex + initialData.Length);

            int lastIndex = Memory.lastIndexOfMemoryVideo - 1;

            // Act
            sut.RemoveFromVideoMemory();

            // Assert
            Assert.Equal(0, Memory.programData[lastIndex]);
        }

        [Fact]
        public void RemoveFromVideoMemory_DoesNotUnderflow()
        {
            // Arrange
            Memory.InitMemory();
            Memory.WipeVideoMemory();
            Memory.lastIndexOfMemoryVideo = Memory.firstVideoMemoryIndex; // Set to the same index as the first video memory index

            // Act
            sut.RemoveFromVideoMemory();

            // Assert
            Assert.Equal(Memory.firstVideoMemoryIndex, Memory.lastIndexOfMemoryVideo); // Should remain the same
        }
    }
}
