using Minimarket.Data;
using Minimarket.DTOs;

namespace Minimarket.UI
{
    public class LowStockReportForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Button btnRefrescar = new Button { Text = "Refrescar", AutoSize = true };
        private Button btnExportarPDF = new Button { Text = "Exportar PDF", AutoSize = true };
        private Label lblInfo = new Label { AutoSize = true, ForeColor = System.Drawing.Color.Red, Font = new Font("Arial", 10, FontStyle.Bold) };

        public LowStockReportForm()
        {
            Text = "Reporte de Productos con Bajo Stock";
            Width = 900; Height = 500;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight
            };

            btnRefrescar.Margin = new Padding(0, 3, 10, 3);
            btnExportarPDF.Margin = new Padding(10, 3, 10, 3);
            lblInfo.Margin = new Padding(20, 6, 0, 3);

            topPanel.Controls.AddRange(new Control[] { btnRefrescar, btnExportarPDF, lblInfo });

            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Code", HeaderText = "Código", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nombre" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Category", HeaderText = "Categoría", Width = 150 });
            
            var colStock = new DataGridViewTextBoxColumn { DataPropertyName = "Stock", HeaderText = "Stock Actual", Width = 100 };
            colStock.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            colStock.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            colStock.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            grid.Columns.Add(colStock);
            
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MinStock", HeaderText = "Stock Mínimo", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Precio", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            Controls.Add(grid);
            Controls.Add(topPanel);

            Load += (s, e) => Cargar();
            btnRefrescar.Click += (s, e) => Cargar();
            btnExportarPDF.Click += (s, e) => ExportarPDF();
        }

        private void Cargar()
        {
            var productos = ProductRepository.GetProductosBajoStock();
            grid.DataSource = productos;
            lblInfo.Text = $"⚠️ {productos.Count} productos con stock bajo o agotado";
            
            if (productos.Count == 0)
            {
                lblInfo.Text = "✅ Todos los productos tienen stock suficiente";
                lblInfo.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblInfo.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void ExportarPDF()
        {
            var productos = grid.DataSource as List<ProductListDto>;
            if (productos == null || productos.Count == 0)
            {
                MessageBox.Show("No hay productos para exportar", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var carpetaReportes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes");
                Directory.CreateDirectory(carpetaReportes);

                var rutaPdf = Path.Combine(carpetaReportes, $"BajoStock_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                Minimarket.Helpers.PdfHelper.GenerarReporteBajoStock(productos, rutaPdf);

                var result = MessageBox.Show($"Reporte generado exitosamente.\n\n¿Desea abrir el PDF?",
                    "Exportación Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = rutaPdf,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
