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
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _exportPath;
        private Timer _timer;

        public DailyFinanceExportService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _exportPath = configuration["ExportPath"] ?? @"FinanceExports";
            ExcelPackage.License.SetNonCommercialPersonal("My Name");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Calculate initial delay until next midnight
                var now = DateTime.Now;
                var nextMidnight = DateTime.Today.AddDays(1);
                var initialDelay = nextMidnight - now;

                // Set up timer to run daily at midnight
                _timer = new Timer(
                    async state => await RunExportAsync(stoppingToken),
                    null,
                    initialDelay, // Time until first run
                    TimeSpan.FromDays(1) // Repeat every 24 hours
                );

                // Keep the service running until cancellation
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when service is stopping
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing service: {ex.Message}");
            }
        }

        private async Task RunExportAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            try
            {
                Console.WriteLine($"Starting export at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                await CleanOldExcelFiles();
                await ExportFinancesToExcel();
                await ExportExpensesToExcel();
                Console.WriteLine($"Export completed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in finance export: {ex.Message}");
            }
        }

        public async Task<Response<object>> DockerBackupDataGenerator()
        {
            try
            {
                await CleanOldExcelFiles();
                await ExportFinancesToExcel();
                await ExportExpensesToExcel();
                return Response<object>.Success(null, "Backup created successfully");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure("Failed to generate backup", ex.ToString());
            }
        }

        private async Task CleanOldExcelFiles()
        {
            if (Directory.Exists(_exportPath))
            {
                var files = Directory.GetFiles(_exportPath, "*.xlsx");
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    fileInfo.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(_exportPath);
            }
        }

        private async Task ExportFinancesToExcel()
        {
            Directory.CreateDirectory(_exportPath);
            using (var scope = _scopeFactory.CreateScope())
            {
                var financeService = scope.ServiceProvider.GetRequiredService<IFinanceService>();

                var financeResponse = await financeService.GetAllFinance();
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

        private async Task ExportExpensesToExcel()
        {
            Directory.CreateDirectory(_exportPath);
            using (var scope = _scopeFactory.CreateScope())
            {
                var financeService = scope.ServiceProvider.GetRequiredService<IExpenseService>();
                var financeResponse = await financeService.GetAllExpense();
                if (!financeResponse.IsSuccess || financeResponse.Result == null)
                {
                    Console.WriteLine($"Failed to get expenses: {financeResponse.Message}");
                    return;
                }
                var expenses = financeResponse.Result;
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var filePath = Path.Combine(_exportPath, $"Expenses_{date}.xlsx");
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Expenses");

                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Date";
                    worksheet.Cells[1, 3].Value = "Expense Type";
                    worksheet.Cells[1, 4].Value = "Amount";
                    worksheet.Cells[1, 5].Value = "Created At";

                    int row = 2;
                    foreach (var expense in expenses)
                    {
                        worksheet.Cells[row, 1].Value = expense.Id;
                        worksheet.Cells[row, 2].Value = expense.Date.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 3].Value = expense.ExpenseType;
                        worksheet.Cells[row, 4].Value = (double)expense.Amount;
                        worksheet.Cells[row, 5].Value = expense.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(cancellationToken);
        }
    }
}