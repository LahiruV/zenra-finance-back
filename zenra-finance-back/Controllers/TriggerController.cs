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

        [HttpPost("ExportFinancesToExcel")]
        public async Task<IActionResult> ExportNow()
        {
            try
            {
                await _exportService.ExportFinancesToExcel();
                return Ok("Finance export triggered successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Export failed: {ex.Message}");
            }
        }
    }
}