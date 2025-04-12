using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zenra_finance_back.Services;

namespace zenra_finance_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TriggerController : ControllerBase
    {
        private readonly DailyFinanceExportService _exportService;

        public TriggerController(DailyFinanceExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpPost("DockerBackupDataGenerator")]
        public async Task<IActionResult> DockerBackupDataGenerator()
        {
            var response = await _exportService.DockerBackupDataGenerator();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}