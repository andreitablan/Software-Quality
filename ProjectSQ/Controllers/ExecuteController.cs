using Microsoft.AspNetCore.Mvc;
using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Interfaces.Processor;

namespace ProjectSQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecuteController : ControllerBase
    {
        private readonly IProcessorService processorService;
        private readonly IParseService parseService;

        public ExecuteController(IProcessorService processorService, IParseService parseService)
        {
            this.processorService = processorService;
            this.parseService = parseService;
        }

        [HttpGet]
        public ActionResult ExecuteFile()
        {
            parseService.LoadInstructions("ProjectSQ.Utils.input.txt");
            processorService.ExecuteFile();
            processorService.ResetData();

            return Ok();
        }
    }
}
