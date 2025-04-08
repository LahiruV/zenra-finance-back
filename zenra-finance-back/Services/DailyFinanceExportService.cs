using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Services
{
    public class DailyFinanceExportService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory; // Replace IFinanceService with this
        private readonly string _exportPath;

        public DailyFinanceExportService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _exportPath = configuration["ExportPath"] ?? @"C:\FinanceExports";
            ExcelPackage.License.SetNonCommercialPersonal("My Name");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var midnight = DateTime.Today.AddDays(1);
                    var delay = midnight - now;

                    await Task.Delay(delay, stoppingToken);
                    CleanOldExcelFiles();
                    await ExportFinancesToExcel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in finance export: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private void CleanOldExcelFiles()
        {
            try
            {
                if (!Directory.Exists(_exportPath))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-2);
                var files = Directory.GetFiles(_exportPath, "Finances_*.xlsx");

                foreach (var file in files)
                {
                    var creationTime = File.GetCreationTime(file);
                    if (creationTime < cutoffDate)
                    {
                        File.Delete(file);
                        Console.WriteLine($"Deleted old finance export: {file}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning old Excel files: {ex.Message}");
            }
        }

        public async Task ExportFinancesToExcel()
        {
            Directory.CreateDirectory(_exportPath);

            // Create a scope to resolve IFinanceService
            using (var scope = _scopeFactory.CreateScope())
            {
                var financeService = scope.ServiceProvider.GetRequiredService<IFinanceService>();

                var financeResponse = await financeService.GetFinance();
                if (!financeResponse.IsSuccess || financeResponse.Result == null)
                {
                    Console.WriteLine($"Failed to get finances: {financeResponse.Message}");
                    return;
                }

                var finances = financeResponse.Result;
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var filePath = Path.Combine(_exportPath, $"Finances_{date}.xlsx");

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Finances");

                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Date";
                    worksheet.Cells[1, 3].Value = "Income Type";
                    worksheet.Cells[1, 4].Value = "Amount";
                    worksheet.Cells[1, 5].Value = "Created At";

                    int row = 2;
                    foreach (var finance in finances)
                    {
                        worksheet.Cells[row, 1].Value = finance.Id;
                        worksheet.Cells[row, 2].Value = finance.Date.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 3].Value = finance.IncomeType;
                        worksheet.Cells[row, 4].Value = (double)finance.Amount;
                        worksheet.Cells[row, 5].Value = finance.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                        row++;
                    }

                    worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, row - 1, 5].AutoFitColumns();

                    worksheet.Cells[row, 3].Value = "Total";
                    worksheet.Cells[row, 4].Formula = $"SUM(D2:D{row - 1})";
                    worksheet.Cells[row, 3, row, 4].Style.Font.Bold = true;

                    await package.SaveAsync();
                }
            }
        }
    }
}