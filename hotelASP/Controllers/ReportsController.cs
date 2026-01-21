using hotelASP.Authorization;
using hotelASP.Models.Reports;
using hotelASP.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IReportExportService _exportService;

        public ReportsController(IReportService reportService, IReportExportService exportService)
        {
            _reportService = reportService;
            _exportService = exportService;
        }

        [HasPermission(PermissionCodes.ReportView)]
        public IActionResult Index()
        {
            var model = new ReportFilterViewModel();
            return View(model);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> IncomeReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var from = dateFrom ?? DateTime.Now.AddMonths(-1);
            var to = dateTo ?? DateTime.Now;

            var report = await _reportService.GenerateIncomeReportAsync(from, to);
            return View(report);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> CustomerReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var from = dateFrom ?? DateTime.Now.AddMonths(-1);
            var to = dateTo ?? DateTime.Now;

            var report = await _reportService.GenerateCustomerReportAsync(from, to);
            return View(report);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> OrderReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var from = dateFrom ?? DateTime.Now.AddMonths(-1);
            var to = dateTo ?? DateTime.Now;

            var report = await _reportService.GenerateOrderReportAsync(from, to);
            return View(report);
        }

        [HttpPost]
        [HasPermission(PermissionCodes.ReportView)]
        public IActionResult GenerateReport(ReportFilterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            return model.ReportType switch
            {
                ReportType.Income => RedirectToAction(nameof(IncomeReport), new { dateFrom = model.DateFrom, dateTo = model.DateTo }),
                ReportType.Customers => RedirectToAction(nameof(CustomerReport), new { dateFrom = model.DateFrom, dateTo = model.DateTo }),
                ReportType.Orders => RedirectToAction(nameof(OrderReport), new { dateFrom = model.DateFrom, dateTo = model.DateTo }),
                _ => RedirectToAction(nameof(Index))
            };
        }

        #region Excel Export Actions

        [HttpGet]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> ExportIncomeReportToExcel(DateTime dateFrom, DateTime dateTo)
        {
            var report = await _reportService.GenerateIncomeReportAsync(dateFrom, dateTo);
            var fileContent = _exportService.ExportIncomeReportToExcel(report);
            var fileName = $"Raport_Przychodow_{dateFrom:yyyy-MM-dd}_{dateTo:yyyy-MM-dd}.xlsx";
            
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> ExportCustomerReportToExcel(DateTime dateFrom, DateTime dateTo)
        {
            var report = await _reportService.GenerateCustomerReportAsync(dateFrom, dateTo);
            var fileContent = _exportService.ExportCustomerReportToExcel(report);
            var fileName = $"Raport_Klientow_{dateFrom:yyyy-MM-dd}_{dateTo:yyyy-MM-dd}.xlsx";
            
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> ExportOrderReportToExcel(DateTime dateFrom, DateTime dateTo)
        {
            var report = await _reportService.GenerateOrderReportAsync(dateFrom, dateTo);
            var fileContent = _exportService.ExportOrderReportToExcel(report);
            var fileName = $"Raport_Zamowien_{dateFrom:yyyy-MM-dd}_{dateTo:yyyy-MM-dd}.xlsx";
            
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        #endregion
    }
}