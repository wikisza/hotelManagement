using hotelASP.Authorization;
using hotelASP.Models.Reports;
using hotelASP.Interfaces;
using hotelASP.Services;
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

        #region Import/Export Actions

        [HttpPost]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> ImportIncomeReport(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Wybierz plik do za³adowania");
                return View("Index", new ReportFilterViewModel());
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                ModelState.AddModelError("", "Nieprawid³owy format pliku. Wymagany format: .xlsx");
                return View("Index", new ReportFilterViewModel());
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    // Przetwarzanie pliku
                    await _reportService.ImportIncomeReportAsync(stream);
                }
                TempData["SuccessMessage"] = "Raport przychodów zosta³ pomyœlnie zaimportowany";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Wyst¹pi³ b³¹d podczas importowania raportu: {ex.Message}");
            }

            return View("Index", new ReportFilterViewModel());
        }

        [HttpPost]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> ImportCustomerReport(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Wybierz plik do za³adowania");
                return View("Index", new ReportFilterViewModel());
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                ModelState.AddModelError("", "Nieprawid³owy format pliku. Wymagany format: .xlsx");
                return View("Index", new ReportFilterViewModel());
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    // Przetwarzanie pliku
                    await _reportService.ImportCustomerReportAsync(stream);
                }
                TempData["SuccessMessage"] = "Raport klientów zosta³ pomyœlnie zaimportowany";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Wyst¹pi³ b³¹d podczas importowania raportu: {ex.Message}");
            }

            return View("Index", new ReportFilterViewModel());
        }

        [HttpPost]
        [HasPermission(PermissionCodes.ReportView)]
        public async Task<IActionResult> ImportOrderReport(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Wybierz plik do za³adowania");
                return View("Index", new ReportFilterViewModel());
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                ModelState.AddModelError("", "Nieprawid³owy format pliku. Wymagany format: .xlsx");
                return View("Index", new ReportFilterViewModel());
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    // Przetwarzanie pliku
                    await _reportService.ImportOrderReportAsync(stream);
                }
                TempData["SuccessMessage"] = "Raport zamówieñ zosta³ pomyœlnie zaimportowany";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Wyst¹pi³ b³¹d podczas importowania raportu: {ex.Message}");
            }

            return View("Index", new ReportFilterViewModel());
        }

        #endregion
    }
}