using Minimarket.Data;
using Minimarket.DTOs;
using System.Text;

namespace Minimarket.UI
{
    public class AdvancedReportsForm : Form
    {
        private TabControl tabControl = new TabControl { Dock = DockStyle.Fill };
        
        public AdvancedReportsForm()
        {
            Text = "Reportes Avanzados";
            Width = 1100; Height = 650;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }
            
            // Crear pestañas
            tabControl.TabPages.Add("ventasPorEmpleado", "Ventas por Empleado");
            tabControl.TabPages.Add("topProductos", "Top Productos");
            tabControl.TabPages.Add("comprasPorProveedor", "Compras por Proveedor");
            tabControl.TabPages.Add("resumenCaja", "Resumen de Caja");
            
            // Configurar cada pestaña
            ConfigurarVentasPorEmpleado(tabControl.TabPages["ventasPorEmpleado"]);
            ConfigurarTopProductos(tabControl.TabPages["topProductos"]);
            ConfigurarComprasPorProveedor(tabControl.TabPages["comprasPorProveedor"]);
            ConfigurarResumenCaja(tabControl.TabPages["resumenCaja"]);
            
            Controls.Add(tabControl);
        }
        
        private void ConfigurarVentasPorEmpleado(TabPage tab)
        {
            var dpDesde = new DateTimePicker { Value = DateTime.Today.AddDays(-30), Width = 120 };
            var dpHasta = new DateTimePicker { Value = DateTime.Today, Width = 120 };
            var btnBuscar = new Button { Text = "Buscar", AutoSize = true };
            var btnExportar = new Button { Text = "Exportar CSV", AutoSize = true };
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            topPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Desde:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) },
                dpDesde,
                new Label { Text = "Hasta:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) },
                dpHasta,
                btnBuscar,
                btnExportar
            });
            
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Usuario", HeaderText = "Empleado", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantidadVentas", HeaderText = "Cantidad de Ventas", Width = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalVendido", HeaderText = "Total Vendido", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            
            tab.Controls.Add(grid);
            tab.Controls.Add(topPanel);
            
            btnBuscar.Click += (s, e) =>
            {
                using var db = new MinimarketContext();
                var desde = dpDesde.Value.Date;
                var hasta = dpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                
                var reporte = db.Ventas
                    .Where(v => v.DateTime >= desde && v.DateTime <= hasta)
                    .GroupBy(v => v.User.FullName)
                    .Select(g => new
                    {
                        Usuario = g.Key,
                        CantidadVentas = g.Count(),
                        TotalVendido = g.Sum(v => v.Total)
                    })
                    .OrderByDescending(x => x.TotalVendido)
                    .ToList();
                
                grid.DataSource = reporte;
            };
            
            btnExportar.Click += (s, e) => ExportarCSV(grid, "VentasPorEmpleado");
        }
        
        private void ConfigurarTopProductos(TabPage tab)
        {
            var dpDesde = new DateTimePicker { Value = DateTime.Today.AddDays(-30), Width = 120 };
            var dpHasta = new DateTimePicker { Value = DateTime.Today, Width = 120 };
            var nudTop = new NumericUpDown { Value = 10, Minimum = 1, Maximum = 100, Width = 60 };
            var btnBuscar = new Button { Text = "Buscar", AutoSize = true };
            var btnExportar = new Button { Text = "Exportar CSV", AutoSize = true };
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            topPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Desde:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) },
                dpDesde,
                new Label { Text = "Hasta:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) },
                dpHasta,
                new Label { Text = "Top:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) },
                nudTop,
                btnBuscar,
                btnExportar
            });
            
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Posicion", HeaderText = "#", Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Codigo", HeaderText = "Código", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Producto", HeaderText = "Producto" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantidadVendida", HeaderText = "Cantidad Vendida", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalGenerado", HeaderText = "Total Generado", Width = 130, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            
            tab.Controls.Add(grid);
            tab.Controls.Add(topPanel);
            
            btnBuscar.Click += (s, e) =>
            {
                using var db = new MinimarketContext();
                var desde = dpDesde.Value.Date;
                var hasta = dpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                var top = (int)nudTop.Value;
                
                var reporte = db.Items
                    .Where(si => si.Sale.DateTime >= desde && si.Sale.DateTime <= hasta)
                    .GroupBy(si => new { si.Product.Code, si.Product.Name })
                    .Select(g => new
                    {
                        Codigo = g.Key.Code,
                        Producto = g.Key.Name,
                        CantidadVendida = g.Sum(si => si.Qty),
                        TotalGenerado = g.Sum(si => si.Subtotal)
                    })
                    .OrderByDescending(x => x.CantidadVendida)
                    .Take(top)
                    .ToList()
                    .Select((x, index) => new
                    {
                        Posicion = index + 1,
                        x.Codigo,
                        x.Producto,
                        x.CantidadVendida,
                        x.TotalGenerado
                    })
                    .ToList();
                
                grid.DataSource = reporte;
            };
            
            btnExportar.Click += (s, e) => ExportarCSV(grid, "TopProductos");
        }
        
        private void ConfigurarComprasPorProveedor(TabPage tab)
        {
            var dpDesde = new DateTimePicker { Value = DateTime.Today.AddDays(-30), Width = 120 };
            var dpHasta = new DateTimePicker { Value = DateTime.Today, Width = 120 };
            var btnBuscar = new Button { Text = "Buscar", AutoSize = true };
            var btnExportar = new Button { Text = "Exportar CSV", AutoSize = true };
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            topPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Desde:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) },
                dpDesde,
                new Label { Text = "Hasta:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) },
                dpHasta,
                btnBuscar,
                btnExportar
            });
            
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Proveedor", HeaderText = "Proveedor", Width = 250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantidadCompras", HeaderText = "Cantidad de Compras", Width = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalComprado", HeaderText = "Total Comprado", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            
            tab.Controls.Add(grid);
            tab.Controls.Add(topPanel);
            
            btnBuscar.Click += (s, e) =>
            {
                using var db = new MinimarketContext();
                var desde = dpDesde.Value.Date;
                var hasta = dpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                
                var reporte = db.Compras
                    .Where(c => c.Date >= desde && c.Date <= hasta)
                    .GroupBy(c => c.Supplier.Name)
                    .Select(g => new
                    {
                        Proveedor = g.Key,
                        CantidadCompras = g.Count(),
                        TotalComprado = g.Sum(c => c.Total)
                    })
                    .OrderByDescending(x => x.TotalComprado)
                    .ToList();
                
                grid.DataSource = reporte;
            };
            
            btnExportar.Click += (s, e) => ExportarCSV(grid, "ComprasPorProveedor");
        }
        
        private void ConfigurarResumenCaja(TabPage tab)
        {
            var dpDesde = new DateTimePicker { Value = DateTime.Today.AddDays(-30), Width = 120 };
            var dpHasta = new DateTimePicker { Value = DateTime.Today, Width = 120 };
            var btnBuscar = new Button { Text = "Buscar", AutoSize = true };
            var btnExportar = new Button { Text = "Exportar CSV", AutoSize = true };
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var lblResumen = new Label { AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold), Padding = new Padding(10) };
            
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            topPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Desde:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) },
                dpDesde,
                new Label { Text = "Hasta:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) },
                dpHasta,
                btnBuscar,
                btnExportar
            });
            
            var resumenPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = System.Drawing.Color.LightYellow };
            resumenPanel.Controls.Add(lblResumen);
            
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Fecha", HeaderText = "Fecha", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Ingresos", HeaderText = "Ingresos", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", ForeColor = System.Drawing.Color.Green } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Egresos", HeaderText = "Egresos", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", ForeColor = System.Drawing.Color.Red } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Saldo", HeaderText = "Saldo", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Font = new Font("Arial", 10, FontStyle.Bold) } });
            
            tab.Controls.Add(grid);
            tab.Controls.Add(resumenPanel);
            tab.Controls.Add(topPanel);
            
            btnBuscar.Click += (s, e) =>
            {
                using var db = new MinimarketContext();
                var desde = dpDesde.Value.Date;
                var hasta = dpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                
                var movimientos = db.MovimientosCaja
                    .Where(m => m.DateTime >= desde && m.DateTime <= hasta)
                    .GroupBy(m => m.DateTime.Date)
                    .Select(g => new
                    {
                        Fecha = g.Key,
                        Ingresos = g.Where(m => m.Type == Constants.CashMovementTypeIn).Sum(m => m.Amount),
                        Egresos = g.Where(m => m.Type == Constants.CashMovementTypeOut).Sum(m => m.Amount)
                    })
                    .ToList()
                    .Select(x => new
                    {
                        x.Fecha,
                        x.Ingresos,
                        x.Egresos,
                        Saldo = x.Ingresos - x.Egresos
                    })
                    .OrderBy(x => x.Fecha)
                    .ToList();
                
                grid.DataSource = movimientos;
                
                var totalIngresos = movimientos.Sum(m => m.Ingresos);
                var totalEgresos = movimientos.Sum(m => m.Egresos);
                var saldoTotal = totalIngresos - totalEgresos;
                
                lblResumen.Text = $"📊 RESUMEN DEL PERÍODO:  Ingresos: {totalIngresos:C2}  |  Egresos: {totalEgresos:C2}  |  Saldo Neto: {saldoTotal:C2}";
                lblResumen.ForeColor = saldoTotal >= 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            };
            
            btnExportar.Click += (s, e) => ExportarCSV(grid, "ResumenCaja");
        }
        
        private void ExportarCSV(DataGridView grid, string nombreBase)
        {
            if (grid.DataSource == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                var carpetaReportes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes");
                Directory.CreateDirectory(carpetaReportes);
                
                var rutaArchivo = Path.Combine(carpetaReportes, $"{nombreBase}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                
                var sb = new StringBuilder();
                var headers = grid.Columns.Cast<DataGridViewColumn>();
                sb.AppendLine(string.Join(";", headers.Select(column => column.HeaderText)));
                
                foreach (DataGridViewRow row in grid.Rows)
                {
                    var cells = row.Cells.Cast<DataGridViewCell>();
                    sb.AppendLine(string.Join(";", cells.Select(cell => cell.Value?.ToString() ?? "")));
                }
                
                File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
                
                var result = MessageBox.Show($"Archivo exportado exitosamente:\n{rutaArchivo}\n\n¿Desea abrir la carpeta?",
                    "Exportación Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", carpetaReportes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
