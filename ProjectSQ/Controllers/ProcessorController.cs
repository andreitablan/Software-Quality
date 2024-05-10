using Microsoft.AspNetCore.Mvc;
using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;

namespace ProjectSQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessorController : ControllerBase
    {
        private readonly IParseService parseService;
        private readonly IProcessorService processorService;
        public ProcessorController(IParseService parseService, IProcessorService processorService)
        {
            this.parseService = parseService;
            this.processorService = processorService;
        }
        [HttpGet]
        public ResultRegisters GetRegisterValues()
        {
            parseService.LoadInstructions("ProjectSQ.Utils.input.txt");
            processorService.ExecuteFile();
            var result = processorService.LoadResultRegisters();
            processorService.ResetData();
            return result;
        }
    }
}
