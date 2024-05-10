using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;
using System.IO;

namespace ProjectSQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoryController : ControllerBase
    {
        private readonly IMemoryService memoryService;
        private readonly IParseService parseService;
        private readonly IProcessorService processorService;

        public MemoryController(IMemoryService memoryService, IParseService parseService, IProcessorService processorService)
        {
            this.memoryService = memoryService;
            this.parseService = parseService;
            this.processorService = processorService;

        }
        [HttpGet]
        public ResultMemory GetMemoryValues()
        {
            parseService.LoadInstructions("ProjectSQ.Utils.input.txt");
            processorService.ExecuteFile();
            var result = memoryService.LoadMemoryData();
            processorService.ResetData();
            return result;
        }
    }
}
