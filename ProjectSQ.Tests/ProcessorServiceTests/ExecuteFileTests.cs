using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;
using Xunit;

namespace ProjectSQ.Tests.ProcessorServiceTests
{
    public class ExecuteTests
    {

        private readonly IProcessorService sut;
        private readonly Mock<IMemoryService> mockMemoryService;


        public ExecuteTests()
        {

            //folosit pentru chestii general valabile la toate testele
            mockMemoryService = new Mock<IMemoryService>();
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns(new ResultMemory(Memory.programData));

            var mockClients = new Mock<IHubClients>();
            var mockAllClients = new Mock<IClientProxy>();
            mockClients.SetupGet(clients => clients.All).Returns(mockAllClients.Object);

            var mockHubContext = new Mock<IHubContext<RealTimeHub>>();
            mockHubContext.SetupGet(hubContext => hubContext.Clients).Returns(mockClients.Object);


            sut = new ProcessorService(mockHubContext.Object, mockMemoryService.Object);
        }

        [Fact]
        //When....Should
        public void When_Calling_ExecuteFile_Should_Result_All_Instruction_used()
        {
            //Arrange
            //Prepare data to be testeds
            //Se poate suprascrie logica din constructor specific pentru test

            Memory.internalMemory[0] = "mov reg1,3";
            Memory.instructionsNumber = 1;
            mockMemoryService.Setup(x => x.LoadMemoryData()).Returns((ResultMemory)null);
            //Act
            //Call method needed
            sut.ExecuteFile();


            //Assert
            //Very result
            Assert.Equal(3, Processor.registerDictionary["reg1"]);
        }
    }
}
