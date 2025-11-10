using System.Text;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Models;

namespace Minimarket.Helpers
{
    public static class PdfHelper
    {
        // Método auxiliar para limpiar caracteres especiales
        private static string LimpiarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;

            var sb = new StringBuilder(texto);
            sb.Replace("á", "a").Replace("Á", "A");
            sb.Replace("é", "e").Replace("É", "E");
            sb.Replace("í", "i").Replace("Í", "I");
            sb.Replace("ó", "o").Replace("Ó", "O");
            sb.Replace("ú", "u").Replace("Ú", "U");
            sb.Replace("ñ", "n").Replace("Ñ", "N");
            sb.Replace("ü", "u").Replace("Ü", "U");
            return sb.ToString();
        }

        public static void GenerarTicketVenta(Sale venta, string rutaArchivo)
        {
            using var writer = new PdfWriter(rutaArchivo);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, iText.Kernel.Geom.PageSize.A7);

            // NO usar fuente, dejar que iText use la predeterminada

            // Encabezado
            document.Add(new Paragraph("MINIMARKET")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(10));

            document.Add(new Paragraph($"Ticket N {venta.Id}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(8));

            document.Add(new Paragraph($"Fecha: {venta.DateTime:dd/MM/yyyy HH:mm}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(6));

            document.Add(new Paragraph($"Cajero: {LimpiarTexto(venta.User.FullName)}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(6));

            document.Add(new Paragraph("-------------------------")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(6));

            // Items
            foreach (var item in venta.SaleItems)
            {
                document.Add(new Paragraph($"{LimpiarTexto(item.Product.Name)}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(4));
                document.Add(new Paragraph($"  {item.Qty} x {item.UnitPrice:C2} = {item.Subtotal:C2}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(4));
            }

            document.Add(new Paragraph("-------------------------")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(6));

            // Total
            document.Add(new Paragraph($"TOTAL: {venta.Total:C2}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(8));

            document.Add(new Paragraph($"Metodo de Pago: {venta.PaymentMethod}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(6));

            document.Add(new Paragraph("Gracias por su compra!")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(6)
                .SetMarginTop(2));
        }

        public static void GenerarActaCierre(CashClose cierre, string rutaArchivo)
        {
            using var writer = new PdfWriter(rutaArchivo);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);

            // Encabezado
            document.Add(new Paragraph("ACTA DE CIERRE Y ARQUEO DE CAJA")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(18)
                .SetMarginBottom(20));

            document.Add(new Paragraph($"Fecha: {cierre.Date:dd/MM/yyyy}")
                .SetFontSize(12));

            document.Add(new Paragraph($"Responsable: {LimpiarTexto(cierre.User.FullName)}")
                .SetFontSize(12)
                .SetMarginBottom(20));

            // Tabla de resumen
            var table = new Table(2).UseAllAvailableWidth();
            table.SetMarginBottom(20);

            table.AddHeaderCell(new Cell().Add(new Paragraph("Concepto").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Monto").SetBold()));

            table.AddCell("Efectivo en Caja");
            table.AddCell(cierre.CashInHand.ToString("C2"));

            table.AddCell("Total POS");
            table.AddCell(cierre.PosTotal.ToString("C2"));

            table.AddCell("Total segun Sistema");
            table.AddCell(cierre.SystemTotal.ToString("C2"));

            table.AddCell(new Cell().Add(new Paragraph("Diferencia").SetBold()));
            table.AddCell(new Cell().Add(new Paragraph(cierre.Difference.ToString("C2")).SetBold()
                .SetFontColor(cierre.Difference != 0 ? ColorConstants.RED : ColorConstants.GREEN)));

            document.Add(table);

            // Observaciones
            if (!string.IsNullOrWhiteSpace(cierre.Notes))
            {
                document.Add(new Paragraph("Observaciones:")
                    .SetBold()
                    .SetFontSize(12));
                document.Add(new Paragraph(LimpiarTexto(cierre.Notes))
                    .SetFontSize(10)
                    .SetMarginBottom(30));
            }

            // Firmas
            document.Add(new Paragraph("\n\n\n"));
            document.Add(new Paragraph("_________________________")
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("Firma del Responsable")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10));
        }

        public static void GenerarReporteBajoStock(List<DTOs.ProductListDto> productos, string rutaArchivo)
        {
            using var writer = new PdfWriter(rutaArchivo);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);

            document.Add(new Paragraph("REPORTE DE PRODUCTOS CON BAJO STOCK")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(16)
                .SetMarginBottom(20));

            document.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFontSize(10)
                .SetMarginBottom(20));

            var table = new Table(5).UseAllAvailableWidth();

            table.AddHeaderCell(new Cell().Add(new Paragraph("Codigo").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Producto").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Categoria").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Stock").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Stock Min.").SetBold()));

            foreach (var p in productos)
            {
                table.AddCell(p.Code);
                table.AddCell(LimpiarTexto(p.Name));
                table.AddCell(LimpiarTexto(p.Category));
                table.AddCell(new Cell().Add(new Paragraph(p.Stock.ToString())
                    .SetFontColor(ColorConstants.RED)));
                table.AddCell(p.MinStock.ToString());
            }

            document.Add(table);

            document.Add(new Paragraph($"\nTotal productos con bajo stock: {productos.Count}")
                .SetBold()
                .SetMarginTop(20));
        }
    }
}
