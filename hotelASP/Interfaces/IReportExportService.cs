using hotelASP.Models.Reports;

namespace hotelASP.Interfaces
{
    public interface IReportExportService
    {
        byte[] ExportIncomeReportToExcel(IncomeReportViewModel report);
        byte[] ExportCustomerReportToExcel(CustomerReportViewModel report);
        byte[] ExportOrderReportToExcel(OrderReportViewModel report);
        byte[] ExportIncomeReportToPdf(IncomeReportViewModel report);
        byte[] ExportCustomerReportToPdf(CustomerReportViewModel report);
        byte[] ExportOrderReportToPdf(OrderReportViewModel report);
    }
}