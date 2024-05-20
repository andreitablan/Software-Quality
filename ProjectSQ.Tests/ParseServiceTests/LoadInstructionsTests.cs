using Xunit;
using FluentAssertions;
using ProjectSQ.Services;
using ProjectSQ.Interfaces.Parser;
using Moq;
using System.IO;
using System.Reflection;
using ProjectSQ.Models;
using ProjectSQ.Interfaces.Memory;

namespace ProjectSQ.Tests.ParseServiceTests
{
    public class LoadInstructionsTests 
    {
        private readonly ParseService _parseService;

        public LoadInstructionsTests()
        {
            IMemoryService memoryService = new MemoryService();

            // Act
            ResultMemory resultMemory = memoryService.LoadMemoryData();
            _parseService = new ParseService();
            ResetData();
        }

        [Fact]
        public void LoadInstructions_FileExists_InstructionsLoaded()
        {
            // Arrange
            var mockStream = new MemoryStream();
            var mockStreamReader = new StreamReader(mockStream);
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetManifestResourceStream(It.IsAny<string>())).Returns(mockStream);
            var mockParserService = new Mock<IParseService>();

            string[] instructions = new string[]
            {
                "main",
                "mov reg1, 5",
                "add reg1, 10"
            };
            foreach (var instruction in instructions)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(instruction + "\n");
                mockStream.Write(bytes, 0, bytes.Length);
            }
            mockStream.Seek(0, SeekOrigin.Begin);

            // Act
            _parseService.LoadInstructions("testfile.txt");

            // Assert
            Memory.internalMemory[0].Should().Be("main");
            Memory.internalMemory[1].Should().Be("mov reg1, 5");
            Memory.internalMemory[2].Should().Be("add reg1, 10");
            Memory.currentInstruction.Should().Be(0);
            Memory.instructionsNumber.Should().Be(3);
            Processor.StackPointer.Should().Be(Memory.startStack);
        }

        [Fact]
        public void LoadInstructions_FileNotExists_InstructionsNotLoaded()
        {
            // Arrange
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetManifestResourceStream(It.IsAny<string>())).Returns((Stream)null);
            var mockParserService = new Mock<IParseService>();

            // Act
            _parseService.LoadInstructions("nonexistentfile.txt");

            // Assert
            Memory.internalMemory.Should().OnlyContain(instruction => string.IsNullOrEmpty(instruction));
            Memory.currentInstruction.Should().Be(0);
            Memory.instructionsNumber.Should().Be(0);
            Processor.StackPointer.Should().Be(Memory.startStack);
        }

        [Fact]
        public void LoadInstructions_EmptyFile_NoInstructionsLoaded()
        {
            // Arrange
            var mockStream = new MemoryStream();
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetManifestResourceStream(It.IsAny<string>())).Returns(mockStream);

            // Act
            _parseService.LoadInstructions("emptyfile.txt");

            // Assert
            Memory.internalMemory.Should().OnlyContain(instruction => string.IsNullOrEmpty(instruction));
            Memory.currentInstruction.Should().Be(0);
            Memory.instructionsNumber.Should().Be(0);
            Processor.StackPointer.Should().Be(Memory.startStack);
        }

        [Fact]
        public void LoadInstructions_FileWithWhitespaceLines_HandlesCorrectly()
        {
            // Arrange
            var mockStream = new MemoryStream();
            var mockStreamReader = new StreamReader(mockStream);
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetManifestResourceStream(It.IsAny<string>())).Returns(mockStream);

            string[] instructions = new string[]
            {
                "main",
                "mov reg1,5",
                "add reg1,10"
            };
            foreach (var instruction in instructions)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(instruction + "\n");
                mockStream.Write(bytes, 0, bytes.Length);
            }
            mockStream.Seek(0, SeekOrigin.Begin);

            // Act
            _parseService.LoadInstructions("whitespacefile.txt");

            // Assert
            Memory.internalMemory[0].Should().Be("main");
            Memory.internalMemory[1].Should().Be("mov reg1,5");
            Memory.internalMemory[2].Should().Be("add reg1,10");
            Memory.currentInstruction.Should().Be(0);
            Memory.instructionsNumber.Should().Be(3);
            Processor.StackPointer.Should().Be(Memory.startStack);
        }



        [Fact]
        public void LoadInstructions_OnlyOneMainLabel_SetsFirstMainAsCurrentInstruction()
        {
            // Act
            _parseService.LoadInstructions("ProjectSQ.Utils.testFileLoadInstructions.txt");

            // Assert
            Memory.internalMemory[0].Should().Be("main");
            Memory.internalMemory[1].Should().Be("mov reg1, 5");
            Memory.internalMemory[2].Should().Be("add reg1, 10");
            Memory.currentInstruction.Should().Be(0);
            Memory.instructionsNumber.Should().Be(3);
            Processor.StackPointer.Should().Be(Memory.startStack);
        }

        [Fact]
        public void LoadInstructions_NullFileInput_HandlesCorrectly()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _parseService.LoadInstructions(null));
        }

        private void ResetData()
        {
            for (int i = 0; i < Memory.internalMemory.Length; i++)
            {
                Memory.internalMemory[i] = string.Empty;
            }
            Memory.currentInstruction = 0;
            Memory.instructionsNumber = 0;
            Processor.StackPointer = Memory.startStack;
        }
    }
}
