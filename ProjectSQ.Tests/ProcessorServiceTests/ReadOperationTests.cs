﻿using Xunit;
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
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOperation", It.IsAny<object>(), default), Times.Once);
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
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOperation", It.IsAny<object>(), default), Times.Once);
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
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOperation", It.IsAny<object>(), default), Times.Once);
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
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOperation", It.IsAny<object>(), default), Times.Once);
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
            _mockHubContext.Verify(hub => hub.Clients.All.SendAsync("ReadOperation", It.IsAny<object>(), default), Times.Once);
        }
    }
}