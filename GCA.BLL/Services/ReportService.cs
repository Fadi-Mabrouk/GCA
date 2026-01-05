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
                    
                    page.Header().Element(header => ComposeHeader(header, sale.Id, sale.Date, sale.Client.Name, sale.Client.Email));
                    page.Content().Element(content => ComposeContent(content, sale.LineItems, sale.TotalAmount));
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

        public void GeneratePurchaseInvoicePdf(Purchase purchase, string filePath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);

                    page.Header().Element(header => ComposeHeader(header, purchase.Id, purchase.Date, purchase.Supplier.Name, null, "Purchase Invoice"));
                    page.Content().Element(content => ComposePurchaseContent(content, purchase.LineItems, purchase.TotalCost));
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

        private void ComposeHeader(IContainer container, int id, DateTime date, string entityName, string? email, string title = "Invoice")
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{title} #{id}").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                    column.Item().Text($"Date: {date:yyyy-MM-dd HH:mm}");
                    column.Item().Text($"{(title.Contains("Purchase") ? "Supplier" : "Client")}: {entityName}");
                    if (!string.IsNullOrEmpty(email))
                    {
                        column.Item().Text($"Email: {email}");
                    }
                });

                row.ConstantItem(100).Height(50).Placeholder(); // Logo Placeholder
            });
        }

        private void ComposeContent(IContainer container, System.Collections.Generic.IEnumerable<SaleLineItem> lineItems, decimal totalAmount)
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

                    foreach (var item in lineItems)
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
                    table.Cell().AlignRight().PaddingTop(10).Text($"{totalAmount:C}").Bold().FontSize(14);
                });
            });
        }

        private void ComposePurchaseContent(IContainer container, System.Collections.Generic.IEnumerable<PurchaseLineItem> lineItems, decimal totalCost)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Item
                        columns.RelativeColumn();  // Cost
                        columns.RelativeColumn();  // Qty
                        columns.RelativeColumn();  // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Item").Bold();
                        header.Cell().AlignRight().Text("Cost").Bold();
                        header.Cell().AlignRight().Text("Qty").Bold();
                        header.Cell().AlignRight().Text("Total").Bold();
                        header.Cell().ColumnSpan(4).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    foreach (var item in lineItems)
                    {
                        var total = item.Quantity * item.UnitCost;
                        table.Cell().Element(CellStyle).Text(item.Part?.Name ?? "Unknown");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitCost:C}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{total:C}");

                        static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }

                    // Grand Total
                    table.Cell().ColumnSpan(3).AlignRight().PaddingTop(10).Text("Grand Total:").Bold();
                    table.Cell().AlignRight().PaddingTop(10).Text($"{totalCost:C}").Bold().FontSize(14);
                });
            });
        }
    }
}
