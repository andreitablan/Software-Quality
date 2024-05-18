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

        [Fact]
        public void LoadMemoryData_ReturnsResultMemoryWithCorrectData()
        {
            // Arrange
            byte[] testData = new byte[] { 1, 2, 3, 4, 5 };
            IMemoryService memoryService = new MemoryService();

            // Act
            ResultMemory resultMemory = memoryService.LoadMemoryData();

            // Assert
            Assert.Equal(testData, resultMemory.Data);
        }
    }
}