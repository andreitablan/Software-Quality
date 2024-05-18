using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class LoadResultRegistersTests
    {
        [Fact]
        public void LoadResultRegisters_ReturnsCorrectResultRegisters()
        {
            // Arrange
            var processorService = CreateProcessorService();

            // Act
            var result = processorService.LoadResultRegisters();

            // Assert
            result.Should().NotBeNull();
            result.Reg1.Should().Be(0);
            result.Reg2.Should().Be(0);
            result.Reg3.Should().Be(0);
            result.Reg4.Should().Be(0);
            result.Reg5.Should().Be(0);
            result.Reg6.Should().Be(0);
            result.Reg7.Should().Be(0);
            result.Reg8.Should().Be(0);
        }

        private ProcessorService CreateProcessorService()
        {
            var mockHubContext = new Mock<IHubContext<RealTimeHub>>();
            var mockMemoryService = new Mock<IMemoryService>();
            var processorService = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            InitializeProcessorRegisters();
            return processorService;
        }

        private void InitializeProcessorRegisters()
        {
            for (int i = 1; i <= 8; i++)
            {
                Processor.registerDictionary[$"reg{i}"] = 0;
            }
        }
    }
}