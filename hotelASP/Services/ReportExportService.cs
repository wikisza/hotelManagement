using ClosedXML.Excel;
using hotelASP.Models.Reports;
using hotelASP.Interfaces;

namespace hotelASP.Services
{
    public class ReportExportService : IReportExportService
    {
        public byte[] ExportIncomeReportToExcel(IncomeReportViewModel report)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Raport Przychodów");

            // Nag³ówek
            worksheet.Cell("A1").Value = "RAPORT PRZYCHODÓW";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;

            worksheet.Cell("A2").Value = $"Okres: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}";

            // Podsumowanie
            worksheet.Cell("A4").Value = "Ca³kowity przychód:";
            worksheet.Cell("B4").Value = report.TotalIncome;
            worksheet.Cell("B4").Style.NumberFormat.Format = "#,##0.00 z³";

            worksheet.Cell("A5").Value = "Przychód z rezerwacji:";
            worksheet.Cell("B5").Value = report.ReservationIncome;
            worksheet.Cell("B5").Style.NumberFormat.Format = "#,##0.00 z³";

            worksheet.Cell("A6").Value = "Przychód z zamówieñ:";
            worksheet.Cell("B6").Value = report.OrdersIncome;
            worksheet.Cell("B6").Style.NumberFormat.Format = "#,##0.00 z³";

            // Podzia³ miesiêczny
            int currentRow = 8;
            worksheet.Cell($"A{currentRow}").Value = "PODZIA£ MIESIÊCZNY";
            worksheet.Cell($"A{currentRow}").Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell($"A{currentRow}").Value = "Miesi¹c";
            worksheet.Cell($"B{currentRow}").Value = "Rezerwacje";
            worksheet.Cell($"C{currentRow}").Value = "Zamówienia";
            worksheet.Cell($"D{currentRow}").Value = "Razem";
            worksheet.Range($"A{currentRow}:D{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:D{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            foreach (var month in report.MonthlyBreakdown)
            {
                worksheet.Cell($"A{currentRow}").Value = month.Month;
                worksheet.Cell($"B{currentRow}").Value = month.ReservationIncome;
                worksheet.Cell($"C{currentRow}").Value = month.OrdersIncome;
                worksheet.Cell($"D{currentRow}").Value = month.TotalIncome;
                worksheet.Range($"B{currentRow}:D{currentRow}").Style.NumberFormat.Format = "#,##0.00 z³";
                currentRow++;
            }

            // Podzia³ wed³ug typu pokoju
            currentRow += 2;
            worksheet.Cell($"A{currentRow}").Value = "PODZIA£ WED£UG TYPU POKOJU";
            worksheet.Cell($"A{currentRow}").Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell($"A{currentRow}").Value = "Typ pokoju";
            worksheet.Cell($"B{currentRow}").Value = "Liczba rezerwacji";
            worksheet.Cell($"C{currentRow}").Value = "Przychód";
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            foreach (var roomType in report.RoomTypeBreakdown)
            {
                worksheet.Cell($"A{currentRow}").Value = roomType.RoomType;
                worksheet.Cell($"B{currentRow}").Value = roomType.ReservationsCount;
                worksheet.Cell($"C{currentRow}").Value = roomType.Income;
                worksheet.Cell($"C{currentRow}").Style.NumberFormat.Format = "#,##0.00 z³";
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportCustomerReportToExcel(CustomerReportViewModel report)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Raport Klientów");

            // Nag³ówek
            worksheet.Cell("A1").Value = "RAPORT KLIENTÓW";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;

            worksheet.Cell("A2").Value = $"Okres: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}";

            // Podsumowanie
            worksheet.Cell("A4").Value = "£¹czna liczba klientów:";
            worksheet.Cell("B4").Value = report.TotalCustomers;

            worksheet.Cell("A5").Value = "Nowi klienci:";
            worksheet.Cell("B5").Value = report.NewCustomers;

            worksheet.Cell("A6").Value = "Powracaj¹cy klienci:";
            worksheet.Cell("B6").Value = report.ReturningCustomers;

            // Top klienci
            int currentRow = 8;
            worksheet.Cell($"A{currentRow}").Value = "TOP KLIENCI";
            worksheet.Cell($"A{currentRow}").Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell($"A{currentRow}").Value = "Imiê i nazwisko";
            worksheet.Cell($"B{currentRow}").Value = "Email";
            worksheet.Cell($"C{currentRow}").Value = "Rezerwacje";
            worksheet.Cell($"D{currentRow}").Value = "Wydano";
            worksheet.Cell($"E{currentRow}").Value = "Ostatnia wizyta";
            worksheet.Range($"A{currentRow}:E{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:E{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            foreach (var customer in report.TopCustomers)
            {
                worksheet.Cell($"A{currentRow}").Value = customer.CustomerName;
                worksheet.Cell($"B{currentRow}").Value = customer.Email;
                worksheet.Cell($"C{currentRow}").Value = customer.ReservationsCount;
                worksheet.Cell($"D{currentRow}").Value = customer.TotalSpent;
                worksheet.Cell($"D{currentRow}").Style.NumberFormat.Format = "#,##0.00 z³";
                worksheet.Cell($"E{currentRow}").Value = customer.LastVisit.ToString("dd.MM.yyyy");
                currentRow++;
            }

            // Miesiêczna statystyka
            currentRow += 2;
            worksheet.Cell($"A{currentRow}").Value = "MIESIÊCZNA STATYSTYKA";
            worksheet.Cell($"A{currentRow}").Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell($"A{currentRow}").Value = "Miesi¹c";
            worksheet.Cell($"B{currentRow}").Value = "Nowi klienci";
            worksheet.Cell($"C{currentRow}").Value = "Wizyty";
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            foreach (var month in report.MonthlyBreakdown)
            {
                worksheet.Cell($"A{currentRow}").Value = month.Month;
                worksheet.Cell($"B{currentRow}").Value = month.NewCustomers;
                worksheet.Cell($"C{currentRow}").Value = month.TotalVisits;
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportOrderReportToExcel(OrderReportViewModel report)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Raport Zamówieñ");

            // Nag³ówek
            worksheet.Cell("A1").Value = "RAPORT ZAMÓWIEÑ";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;

            worksheet.Cell("A2").Value = $"Okres: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}";

            // Podsumowanie
            worksheet.Cell("A4").Value = "£¹czna liczba zamówieñ:";
            worksheet.Cell("B4").Value = report.TotalOrders;

            worksheet.Cell("A5").Value = "Wartoœæ zamówieñ:";
            worksheet.Cell("B5").Value = report.TotalOrdersValue;
            worksheet.Cell("B5").Style.NumberFormat.Format = "#,##0.00 z³";

            worksheet.Cell("A6").Value = "Œrednia wartoœæ:";
            worksheet.Cell("B6").Value = report.AverageOrderValue;
            worksheet.Cell("B6").Style.NumberFormat.Format = "#,##0.00 z³";

            // Najpopularniejsze pozycje
            int currentRow = 8;
            worksheet.Cell($"A{currentRow}").Value = "NAJPOPULARNIEJSZE POZYCJE";
            worksheet.Cell($"A{currentRow}").Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell($"A{currentRow}").Value = "Nazwa";
            worksheet.Cell($"B{currentRow}").Value = "Zamówienia";
            worksheet.Cell($"C{currentRow}").Value = "Przychód";
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            foreach (var item in report.PopularItems)
            {
                worksheet.Cell($"A{currentRow}").Value = item.ItemName;
                worksheet.Cell($"B{currentRow}").Value = item.OrderCount;
                worksheet.Cell($"C{currentRow}").Value = item.Revenue;
                worksheet.Cell($"C{currentRow}").Style.NumberFormat.Format = "#,##0.00 z³";
                currentRow++;
            }

            // Zamówienia wed³ug statusu
            currentRow += 2;
            worksheet.Cell($"A{currentRow}").Value = "ZAMÓWIENIA WED£UG STATUSU";
            worksheet.Cell($"A{currentRow}").Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell($"A{currentRow}").Value = "Status";
            worksheet.Cell($"B{currentRow}").Value = "Liczba";
            worksheet.Cell($"C{currentRow}").Value = "Wartoœæ";
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"A{currentRow}:C{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow++;

            foreach (var status in report.OrdersByStatus)
            {
                worksheet.Cell($"A{currentRow}").Value = status.Status;
                worksheet.Cell($"B{currentRow}").Value = status.Count;
                worksheet.Cell($"C{currentRow}").Value = status.TotalValue;
                worksheet.Cell($"C{currentRow}").Style.NumberFormat.Format = "#,##0.00 z³";
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        #region PDF Export Methods (Placeholder)

        public byte[] ExportIncomeReportToPdf(IncomeReportViewModel report)
        {
            // Tymczasowo zwróæ pusty PDF z informacj¹
            var message = $"Eksport PDF bêdzie dostêpny wkrótce.\nRaport przychodów: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}";
            return System.Text.Encoding.UTF8.GetBytes(message);
        }

        public byte[] ExportCustomerReportToPdf(CustomerReportViewModel report)
        {
            var message = $"Eksport PDF bêdzie dostêpny wkrótce.\nRaport klientów: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}";
            return System.Text.Encoding.UTF8.GetBytes(message);
        }

        public byte[] ExportOrderReportToPdf(OrderReportViewModel report)
        {
            var message = $"Eksport PDF bêdzie dostêpny wkrótce.\nRaport zamówieñ: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}";
            return System.Text.Encoding.UTF8.GetBytes(message);
        }

        #endregion
    }
}