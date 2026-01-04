using System;
using System.IO;
using GCA.BLL.Interfaces;
using GCA.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GCA.BLL.Services
{
    public class ReportService : IReportService
    {
        static ReportService()
        {
            // Configure License for Community or Pro. 
            // Using Community to match open-source/free usage.
            // CAUTION: User must ensure they comply with QuestPDF license.
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GenerateInvoicePdf(Sale sale, string filePath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    
                    page.Header().Element(header => ComposeHeader(header, sale));
                    page.Content().Element(content => ComposeContent(content, sale));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            })
            .GeneratePdf(filePath);
        }

        private void ComposeHeader(IContainer container, Sale sale)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Invoice #{sale.Id}").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                    column.Item().Text($"Date: {sale.Date:yyyy-MM-dd HH:mm}");
                    column.Item().Text($"Client: {sale.Client.Name}");
                    column.Item().Text($"Email: {sale.Client.Email ?? "N/A"}");
                });

                row.ConstantItem(100).Height(50).Placeholder(); // Logo Placeholder
            });
        }

        private void ComposeContent(IContainer container, Sale sale)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Item
                        columns.RelativeColumn();  // Price
                        columns.RelativeColumn();  // Qty
                        columns.RelativeColumn();  // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Item").Bold();
                        header.Cell().AlignRight().Text("Price").Bold();
                        header.Cell().AlignRight().Text("Qty").Bold();
                        header.Cell().AlignRight().Text("Total").Bold();
                        header.Cell().ColumnSpan(4).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    foreach (var item in sale.LineItems)
                    {
                        var total = item.Quantity * item.UnitPriceSnapshot;
                        table.Cell().Element(CellStyle).Text(item.Part?.Name ?? "Unknown");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPriceSnapshot:C}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{total:C}");
                        
                        static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }

                    // Grand Total
                    table.Cell().ColumnSpan(3).AlignRight().PaddingTop(10).Text("Grand Total:").Bold();
                    table.Cell().AlignRight().PaddingTop(10).Text($"{sale.TotalAmount:C}").Bold().FontSize(14);
                });
            });
        }
    }
}
