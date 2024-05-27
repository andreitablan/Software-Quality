using Xunit;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;
using ProjectSQ.Services;

namespace ProjectSQ.Tests.MemoryServiceTests
{
    public class LoadMemoryDataTests
    {
        [Fact]
        public void LoadMemoryData_ReturnsNonNullResultMemory()
        {
            // Arrange
            byte[] testData = new byte[] { 1, 2, 3, 4, 5 };
            IMemoryService memoryService = new MemoryService();

            // Act
            ResultMemory resultMemory = memoryService.LoadMemoryData();

            // Assert
            Assert.NotNull(resultMemory);
        }

        //Fails because ResultMemory constructor attempted to access an index beyond the bounds of the input array memory
        [Fact]
        public void LoadMemoryData_ReturnsResultMemoryWithCorrectData()
        {
            // Arrange
            byte[] testData = new byte[] { 1, 2, 3, 4, 5};
            Memory.programData = testData;
            IMemoryService memoryService = new MemoryService();

            // Act
            ResultMemory resultMemory = memoryService.LoadMemoryData();

            // Assert
            Assert.NotNull(resultMemory.NonZeroValues);
            Assert.Equal(5, resultMemory.NonZeroValues.Count);
        }
    }
}