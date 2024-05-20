using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;
using System;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class ExecuteTests
    {

        private readonly IProcessorService sut;
        private readonly Mock<IMemoryService> mockMemoryService;

        public ExecuteTests()
        {
            mockMemoryService = new Mock<IMemoryService>();
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns(new ResultMemory(Memory.programData));

            var mockClients = new Mock<IHubClients>();
            var mockAllClients = new Mock<IClientProxy>();
            mockClients.SetupGet(clients => clients.All).Returns(mockAllClients.Object);

            var mockHubContext = new Mock<IHubContext<RealTimeHub>>();
            mockHubContext.SetupGet(hubContext => hubContext.Clients).Returns(mockClients.Object);


            sut = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
            sut.ResetData();
        }

        [Fact]
        //When....Should
        public void When_Calling_ExecuteFile_Should_Result_Assignment()
        {
            //Arrange
            //Prepare data to be testeds
            //Se poate suprascrie logica din constructor specific pentru test

            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,3";
            Memory.instructionsNumber = 2;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            //Call method needed
            sut.ExecuteFile();


            //Assert
            //Very result
            Processor.registerDictionary["reg1"].Should().Be(3);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Addition()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,27033";
            Memory.internalMemory[2] = "mov reg2,27000";
            Memory.internalMemory[3] = "add reg1,reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(54033);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Subtraction()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,54033";
            Memory.internalMemory[2] = "mov reg2,27033";
            Memory.internalMemory[3] = "sub reg1,reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(27000);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Multiplication()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,25000";
            Memory.internalMemory[2] = "mov reg2,2";
            Memory.internalMemory[3] = "mul reg1,reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(50000);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Division()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,50000";
            Memory.internalMemory[2] = "mov reg2,2";
            Memory.internalMemory[3] = "div reg1,reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(25000);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Not()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,10";
            Memory.internalMemory[2] = "not reg1";
            Memory.instructionsNumber = 3;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(unchecked((ushort)~10));
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_And()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,10";
            Memory.internalMemory[2] = "mov reg2,6";
            Memory.internalMemory[3] = "and reg1,reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(10 & 6);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Or()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,10";
            Memory.internalMemory[2] = "mov reg2,6";
            Memory.internalMemory[3] = "or reg1,reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(10 | 6);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Xor()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,10";
            Memory.internalMemory[2] = "mov reg2,6";
            Memory.internalMemory[3] = "xor reg1,reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(10 ^ 6);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Push()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,54033";
            Memory.internalMemory[2] = "push reg1";
            Memory.instructionsNumber = 3;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.StackPointer.Should().Be(2);
            byte lowByte = Memory.programData[Processor.StackPointer - 2];
            byte highByte = Memory.programData[Processor.StackPointer - 1];
            ushort result = (ushort)((highByte << 8) | lowByte);

            result.Should().Be(54033);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_Pop()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,54033";
            Memory.internalMemory[2] = "push reg1";
            Memory.internalMemory[3] = "pop reg2";
            Memory.instructionsNumber = 4;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg2"].Should().Be(54033);
            Processor.StackPointer.Should().Be(0);
            Memory.programData[Processor.StackPointer].Should().Be(0);
            Memory.programData[Processor.StackPointer + 1].Should().Be(0);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_LeftShift()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,54033";
            Memory.internalMemory[2] = "shl reg1,2";
            Memory.instructionsNumber = 3;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(unchecked((ushort)(54033 << 2)));
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_RightShift()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,54033";
            Memory.internalMemory[2] = "shr reg1,2";
            Memory.instructionsNumber = 3;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            ushort result = 54033 >> 2;
            Processor.registerDictionary["reg1"].Should().Be(result);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_FunctionCall()
        {
            //Arrange
            Memory.internalMemory[0] = "function testMethod";
            Memory.internalMemory[1] = "mov reg2,27033";
            Memory.internalMemory[2] = "return";
            Memory.internalMemory[3] = "main";
            Memory.internalMemory[4] = "call testMethod";
            Memory.internalMemory[5] = "mov reg1,54033";
        
            Memory.instructionsNumber = 6;
            Memory.currentInstruction = 3;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(54033);
            Processor.registerDictionary["reg2"].Should().Be(27033);
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_CompareAndUnconditionalJump()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,10";
            Memory.internalMemory[2] = "cmp reg1,8";
            Memory.internalMemory[3] = "jl maiMic";
            Memory.internalMemory[4] = "jmp final";
            Memory.internalMemory[5] = "label maiMic";
            Memory.internalMemory[6] = "mov reg2,4";
            Memory.internalMemory[7] = "label final";
            Memory.internalMemory[8] = "mov reg3,20";
            Memory.instructionsNumber = 9;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(10);
            Processor.registerDictionary["reg2"].Should().Be(0);
            Processor.registerDictionary["reg3"].Should().Be(20);

            Processor.Equal.Should().BeFalse();
            Processor.LessEqual.Should().BeFalse();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeTrue();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeTrue();
        }
        //create a test for the compare instruction and jump to a nonexistent label instruction using the execute file method
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_CompareAndNonexistentJump()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[0] = "mov reg1,10";
            Memory.internalMemory[1] = "jmp final";
            Memory.internalMemory[2] = "cmp reg1,8";
            Memory.instructionsNumber = 3;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            
            //Act
            sut.ExecuteFile();
            
            //Assert
            Processor.Equal.Should().BeFalse();
            Processor.LessEqual.Should().BeFalse();
            Processor.GreaterEqual.Should().BeFalse();

            Processor.NotEqual.Should().BeFalse();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeFalse();
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_JumpIfEqual()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,10";
            Memory.internalMemory[2] = "cmp reg1,10";
            Memory.internalMemory[3] = "je test";
            Memory.internalMemory[4] = "jmp final";
            Memory.internalMemory[5] = "label test";
            Memory.internalMemory[6] = "mov reg3,20";
            Memory.internalMemory[7] = "label final";
            Memory.instructionsNumber = 8;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(10);
            Processor.registerDictionary["reg2"].Should().Be(0);
            Processor.registerDictionary["reg3"].Should().Be(20);

            Processor.Equal.Should().BeTrue();
            Processor.LessEqual.Should().BeTrue();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeFalse();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeFalse();
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_JumpIfLessThanOrEqual()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,10";
            Memory.internalMemory[2] = "cmp reg1,10";
            Memory.internalMemory[3] = "jle test";
            Memory.internalMemory[4] = "jmp final";
            Memory.internalMemory[5] = "label test";
            Memory.internalMemory[6] = "mov reg3,20";
            Memory.internalMemory[7] = "label final";
            Memory.instructionsNumber = 8;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(10);
            Processor.registerDictionary["reg2"].Should().Be(0);
            Processor.registerDictionary["reg3"].Should().Be(20);

            Processor.Equal.Should().BeTrue();
            Processor.LessEqual.Should().BeTrue();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeFalse();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeFalse();
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_JumpIfNotEqual()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,20";
            Memory.internalMemory[2] = "cmp reg1,10";
            Memory.internalMemory[3] = "jne test";
            Memory.internalMemory[4] = "jmp final";
            Memory.internalMemory[5] = "label test";
            Memory.internalMemory[6] = "mov reg3,20";
            Memory.internalMemory[7] = "label final";
            Memory.instructionsNumber = 8;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(20);
            Processor.registerDictionary["reg2"].Should().Be(0);
            Processor.registerDictionary["reg3"].Should().Be(20);

            Processor.Equal.Should().BeFalse();
            Processor.LessEqual.Should().BeFalse();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeTrue();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeTrue();
        }
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_JumpIfGreaterThanOrEqual()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,20";
            Memory.internalMemory[2] = "cmp reg1,10";
            Memory.internalMemory[3] = "jge test";
            Memory.internalMemory[4] = "jmp final";
            Memory.internalMemory[5] = "label test";
            Memory.internalMemory[6] = "mov reg3,20";
            Memory.internalMemory[7] = "label final";
            Memory.instructionsNumber = 8;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(20);
            Processor.registerDictionary["reg2"].Should().Be(0);
            Processor.registerDictionary["reg3"].Should().Be(20);

            Processor.Equal.Should().BeFalse();
            Processor.LessEqual.Should().BeFalse();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeTrue();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeTrue();
        }
        //create a test for the JumpIfGreaterThan instruction using the execute file method
        [Fact]
        public void When_Calling_ExecuteFile_Should_Result_JumpIfGreaterThan()
        {
            //Arrange
            Memory.internalMemory[0] = "main";
            Memory.internalMemory[1] = "mov reg1,20";
            Memory.internalMemory[2] = "cmp reg1,10";
            Memory.internalMemory[3] = "jg test";
            Memory.internalMemory[4] = "jmp final";
            Memory.internalMemory[5] = "label test";
            Memory.internalMemory[6] = "mov reg3,20";
            Memory.internalMemory[7] = "label final";
            Memory.instructionsNumber = 8;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            sut.ExecuteFile();
            //Assert
            Processor.registerDictionary["reg1"].Should().Be(20);
            Processor.registerDictionary["reg2"].Should().Be(0);
            Processor.registerDictionary["reg3"].Should().Be(20);

            Processor.Equal.Should().BeFalse();
            Processor.LessEqual.Should().BeFalse();
            Processor.GreaterEqual.Should().BeTrue();

            Processor.NotEqual.Should().BeTrue();
            Processor.Less.Should().BeFalse();
            Processor.Greater.Should().BeTrue();
        }
    }
}
