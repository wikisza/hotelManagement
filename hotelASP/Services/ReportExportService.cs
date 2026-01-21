using ClosedXML.Excel;
using hotelASP.Models.Reports;
using hotelASP.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace hotelASP.Services
{
    public class ReportExportService : IReportExportService
    {
        public ReportExportService()
        {
            // Konfiguracja QuestPDF - wymagane dla wersji Community
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #region Excel Export Methods

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

            // Formatowanie
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

        #endregion

        #region PDF Export Methods

        public byte[] ExportIncomeReportToPdf(IncomeReportViewModel report)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(HeaderStyle).Text("RAPORT PRZYCHODÓW").Bold().FontSize(20);

                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Okres: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}");
                        column.Item().PaddingVertical(10);

                        // Podsumowanie
                        column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(summary =>
                        {
                            summary.Item().Text($"Ca³kowity przychód: {report.TotalIncome:C}").Bold();
                            summary.Item().Text($"Przychód z rezerwacji: {report.ReservationIncome:C}");
                            summary.Item().Text($"Przychód z zamówieñ: {report.OrdersIncome:C}");
                            summary.Item().Text($"Liczba rezerwacji: {report.TotalReservations}");
                            summary.Item().Text($"Liczba zamówieñ: {report.TotalOrders}");
                        });

                        column.Item().PaddingVertical(10);

                        // Tabela miesiêczna
                        column.Item().Text("Podzia³ miesiêczny").Bold().FontSize(14);
                        column.Item().PaddingVertical(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Miesi¹c").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Rezerwacje").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Zamówienia").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Razem").Bold();
                            });

                            foreach (var month in report.MonthlyBreakdown)
                            {
                                table.Cell().Padding(5).Text(month.Month);
                                table.Cell().Padding(5).Text($"{month.ReservationIncome:C}");
                                table.Cell().Padding(5).Text($"{month.OrdersIncome:C}");
                                table.Cell().Padding(5).Text($"{month.TotalIncome:C}").Bold();
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Strona ");
                        text.CurrentPageNumber();
                        text.Span(" z ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] ExportCustomerReportToPdf(CustomerReportViewModel report)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(HeaderStyle).Text("RAPORT KLIENTÓW").Bold().FontSize(20);

                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Okres: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}");
                        column.Item().PaddingVertical(10);

                        // Podsumowanie
                        column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(summary =>
                        {
                            summary.Item().Text($"£¹czna liczba klientów: {report.TotalCustomers}").Bold();
                            summary.Item().Text($"Nowi klienci: {report.NewCustomers}");
                            summary.Item().Text($"Powracaj¹cy klienci: {report.ReturningCustomers}");
                        });

                        column.Item().PaddingVertical(10);

                        // Top klienci
                        column.Item().Text("Top klienci").Bold().FontSize(14);
                        column.Item().PaddingVertical(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Imiê i nazwisko").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Email").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Rezerwacje").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Wydano").Bold();
                            });

                            foreach (var customer in report.TopCustomers)
                            {
                                table.Cell().Padding(5).Text(customer.CustomerName);
                                table.Cell().Padding(5).Text(customer.Email);
                                table.Cell().Padding(5).Text(customer.ReservationsCount.ToString());
                                table.Cell().Padding(5).Text($"{customer.TotalSpent:C}");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Strona ");
                        text.CurrentPageNumber();
                        text.Span(" z ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] ExportOrderReportToPdf(OrderReportViewModel report)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(HeaderStyle).Text("RAPORT ZAMÓWIEÑ").Bold().FontSize(20);

                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Okres: {report.DateFrom:dd.MM.yyyy} - {report.DateTo:dd.MM.yyyy}");
                        column.Item().PaddingVertical(10);

                        // Podsumowanie
                        column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(summary =>
                        {
                            summary.Item().Text($"£¹czna liczba zamówieñ: {report.TotalOrders}").Bold();
                            summary.Item().Text($"Wartoœæ zamówieñ: {report.TotalOrdersValue:C}");
                            summary.Item().Text($"Œrednia wartoœæ: {report.AverageOrderValue:C}");
                        });

                        column.Item().PaddingVertical(10);

                        // Najpopularniejsze pozycje
                        column.Item().Text("Najpopularniejsze pozycje").Bold().FontSize(14);
                        column.Item().PaddingVertical(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nazwa").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Zamówienia").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Przychód").Bold();
                            });

                            foreach (var item in report.PopularItems)
                            {
                                table.Cell().Padding(5).Text(item.ItemName);
                                table.Cell().Padding(5).Text(item.OrderCount.ToString());
                                table.Cell().Padding(5).Text($"{item.Revenue:C}");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Strona ");
                        text.CurrentPageNumber();
                        text.Span(" z ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static IContainer HeaderStyle(IContainer container)
        {
            return container.Background(Colors.Blue.Lighten3).Padding(10);
        }

        #endregion
    }
}