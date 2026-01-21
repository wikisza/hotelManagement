using System.IO;
using System.Threading.Tasks;
using hotelASP.Models.Reports;

namespace hotelASP.Interfaces
{
    public interface IReportService
    {
        Task<IncomeReportViewModel> GenerateIncomeReportAsync(DateTime dateFrom, DateTime dateTo);
        Task<CustomerReportViewModel> GenerateCustomerReportAsync(DateTime dateFrom, DateTime dateTo);
        Task<OrderReportViewModel> GenerateOrderReportAsync(DateTime dateFrom, DateTime dateTo);

        // Opcjonalne metody importu (jeœli potrzebne w przysz³oœci)
        Task ImportIncomeReportAsync(Stream stream);
        Task ImportCustomerReportAsync(Stream stream);
        Task ImportOrderReportAsync(Stream stream);
    }
}