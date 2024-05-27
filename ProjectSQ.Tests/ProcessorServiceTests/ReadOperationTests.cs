using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Memory;
using Moq;
using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Models;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class ReadOperationTests
    {
        private readonly ProcessorService _processorService;
        private readonly Mock<IHubContext<RealTimeHub>> _mockHubContext;

        public ReadOperationTests()
        {
            _mockHubContext = new Mock<IHubContext<RealTimeHub>>();
            Mock<IMemoryService> mockMemoryService = new Mock<IMemoryService>();
            _processorService = new ProcessorService(_mockHubContext.Object, mockMemoryService.Object);
            _processorService.ResetData();

            // Initialize Processor's register dictionary
            Processor.registerDictionary = new Dictionary<string, ushort> {
                {"reg1" , 0 }, {"reg2" , 0 }, {"reg3" , 0 },{"reg4" , 0 }, {"reg5" , 0 },
                {"reg6" , 0 }, {"reg7" , 0 }, {"reg8" , 0 }
            };
        }

        [Fact]
        public void Read_SingleDigitValue_StoresCorrectlyInRegister()
        {
            // Arrange
            const string register = "reg1";
            Memory.programData = new byte[] { (byte)'5', (byte)' ', 0, 0, 0 };
            Memory.currentIndexMemoryVideo = 0;

            // Act
            _processorService.Read(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(5);
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOpearion", It.IsAny<object>(), default), Times.Once);
        }

        [Fact]
        public void Read_MultiDigitValue_StoresCorrectlyInRegister()
        {
            // Arrange
            const string register = "reg1";
            Memory.programData = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)' ', 0 };
            Memory.currentIndexMemoryVideo = 0;

            // Act
            _processorService.Read(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(123);
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOpearion", It.IsAny<object>(), default), Times.Once);
        }

        [Fact]
        public void Read_ValueWithLeadingSpaces_SkipsSpacesAndStoresCorrectly()
        {
            // Arrange
            const string register = "reg1";
            Memory.programData = new byte[] { (byte)' ', (byte)' ', (byte)'4', (byte)'5', (byte)' ', 0 };
            Memory.currentIndexMemoryVideo = 2;

            // Act
            _processorService.Read(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(45);
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOpearion", It.IsAny<object>(), default), Times.Once);
        }

        [Fact]
        public void Read_MultipleValues_ReadsFirstValueCorrectly()
        {
            // Arrange
            const string register = "reg1";
            Memory.programData = new byte[] { (byte)'7', (byte)'8', (byte)' ', (byte)'9', (byte)'0', (byte)' ', 0 };
            Memory.currentIndexMemoryVideo = 0;

            // Act
            _processorService.Read(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(78);
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOpearion", It.IsAny<object>(), default), Times.Once);
        }

        [Fact]
        public void Read_EmptyMemory_DoesNotCrash()
        {
            // Arrange
            const string register = "reg1";
            Memory.programData = new byte[5];
            Memory.currentIndexMemoryVideo = 0;

            // Act
            _processorService.Read(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(0);
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOpearion", It.IsAny<object>(), default), Times.Once);
        }

        [Fact]
        public void Read_NoSpaceAfterDigits_StoresCorrectlyInRegister()
        {
            // Arrange
            const string register = "reg1";
            Memory.programData = new byte[] { (byte)'4', (byte)'5', 0, 0, 0 };
            Memory.currentIndexMemoryVideo = 0;

            // Act
            _processorService.Read(register);

            // Assert
            Processor.registerDictionary[register].Should().Be(45);
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOpearion", It.IsAny<object>(), default), Times.Once);
        }
    }
}
