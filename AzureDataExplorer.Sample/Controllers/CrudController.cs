using Microsoft.AspNetCore.Mvc;

namespace AzureDataExplorer.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrudController : ControllerBase
    {
        private readonly ILogger<CrudController> _logger;
        private readonly ILogsService _logsService;

        public CrudController(ILogger<CrudController> logger, ILogsService logsService)
        {
            _logger = logger;
            _logsService = logsService;
        }

        [HttpPost("CreateTable")]
        public async Task<IActionResult> CreateTable()
        {
            await _logsService.CreateTableAsync();

            return Ok();
        }

        [HttpPost("DropTable")]
        public async Task<IActionResult> DropTable()
        {
            await _logsService.DropTableAsync();

            return Ok();
        }

        [HttpPost("IngestInline")]
        public async Task<IActionResult> IngestInline(string type, string message)
        {
            await _logsService.IngestInlineAsync(type, message);

            return Ok();
        }

        [HttpGet("Count")]
        public async Task<IActionResult> Count(string type)
        {
            var res = await _logsService.CountAsync(type);

            return Ok(res);
        }


        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var res = await _logsService.GetRecords();

            return Ok(res);
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _logsService.DeleteRecordsAsync(id);

            return Ok();
        }

        [HttpPost("ClearAllData")]
        public async Task<IActionResult> ClearAllData()
        {
            await _logsService.ClearAllRecords();

            return Ok();
        }
    }
}