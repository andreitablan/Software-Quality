using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests
{
    public class WriteToVideoMemoryTests
    {

        [Fact]
        public void WriteToVideoMemory_ShouldWriteKeyboardBufferValueToVideoMemory()
        {
            // Arrange
            Memory.InitMemory();
            Memory.programData[Memory.keyboardBufferIndex] = (byte)'A'; // ASCII value of 'A'
            Memory.isKeyboardBufferChanged = true;

            // Act
            Task.Run(ProcessorService.WriteToVideoMemory).Wait(100);

            // Assert
            Assert.Equal((byte)'A', Memory.programData[Memory.lastIndexOfMemoryVideo - 1]);
        }

        [Fact]
        public void WriteToVideoMemory_ShouldUpdateLastIndexOfMemoryVideo()
        {
            // Arrange
            Memory.InitMemory();
            Memory.programData[Memory.keyboardBufferIndex] = (byte)'A';
            Memory.isKeyboardBufferChanged = true;
            ushort initialLastIndex = Memory.lastIndexOfMemoryVideo;

            // Act
            Task.Run(ProcessorService.WriteToVideoMemory).Wait(100);

            // Assert
            Assert.Equal(initialLastIndex + 1, Memory.lastIndexOfMemoryVideo);
        }

        [Fact]
        public void WriteToVideoMemory_ShouldNotExceedMaxIndexOfMemoryVideo()
        {
            // Arrange
            Memory.InitMemory();
            Memory.lastIndexOfMemoryVideo = Memory.maxIndexOfMemoryVideo;
            Memory.programData[Memory.keyboardBufferIndex] = (byte)'A';
            Memory.isKeyboardBufferChanged = true;

            // Act
            Task.Run(ProcessorService.WriteToVideoMemory).Wait(100);

            // Assert
            Assert.Equal(Memory.maxIndexOfMemoryVideo, Memory.lastIndexOfMemoryVideo);
            Assert.Equal((byte)'A', Memory.programData[Memory.maxIndexOfMemoryVideo]);
        }

        [Fact]
        public void WriteToVideoMemory_ShouldResetIsKeyboardBufferChangedFlag()
        {
            // Arrange
            Memory.InitMemory();
            Memory.programData[Memory.keyboardBufferIndex] = (byte)'A';
            Memory.isKeyboardBufferChanged = true;

            // Act
            Task.Run(ProcessorService.WriteToVideoMemory).Wait(100);

            // Assert
            Assert.False(Memory.isKeyboardBufferChanged);
        }

        [Fact]
        public void WriteToVideoMemory_ShouldNotAlterLastIndexWhenBufferNotChanged()
        {
            // Arrange
            Memory.InitMemory();
            ushort initialLastIndex = Memory.lastIndexOfMemoryVideo;

            // Act
            Task.Run(ProcessorService.WriteToVideoMemory).Wait(100); // Run the method for a short time

            // Assert
            Assert.Equal(initialLastIndex, Memory.lastIndexOfMemoryVideo);

            // Cleanup
            Memory.StopWriteToVideoMemory = true; // Stop the loop
        }
    }
}
