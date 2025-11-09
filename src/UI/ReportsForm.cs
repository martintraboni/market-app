using System;
using System.Windows.Forms;
using System.Linq;
using Minimarket.Data;

namespace Minimarket.UI
{
    public class ReportsForm : Form
    {
        private DateTimePicker dpDesde = new DateTimePicker { Value = DateTime.Today.AddDays(-7), Width = 120 };
        private DateTimePicker dpHasta = new DateTimePicker { Value = DateTime.Today, Width = 120 };
        private Button btnBuscar = new Button { Text = "Buscar", AutoSize = true };
        private Button btnExportar = new Button { Text = "Exportar a CSV", AutoSize = true };
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Label lblTotal = new Label { AutoSize = true, Font = new Font("Arial", 11, FontStyle.Bold), ForeColor = System.Drawing.Color.Green };
        private Label lblCantidad = new Label { AutoSize = true, Font = new Font("Arial", 10, FontStyle.Regular) };

        public ReportsForm()
        {
            Text = "Reportes de Ventas";
            Width = 1000; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Panel superior con controles alineados
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            topPanel.Controls.Add(new Label { Text = "Desde:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) });
            topPanel.Controls.Add(dpDesde);
            topPanel.Controls.Add(new Label { Text = "Hasta:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            topPanel.Controls.Add(dpHasta);
            btnBuscar.Margin = new Padding(15, 3, 5, 3);
            topPanel.Controls.Add(btnBuscar);

            // Espaciador
            topPanel.Controls.Add(new Panel { Width = 100, Height = 1 });

            btnExportar.Margin = new Padding(15, 3, 5, 3);
            topPanel.Controls.Add(btnExportar);

            // Panel de resumen
            var resumenPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = System.Drawing.Color.LightYellow
            };

            resumenPanel.Controls.Add(lblCantidad);
            resumenPanel.Controls.Add(new Label { Text = "  |  ", AutoSize = true, Margin = new Padding(10, 0, 10, 0) });
            resumenPanel.Controls.Add(lblTotal);

            // Configurar grid con columnas en español
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "N° Venta", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Fecha/Hora", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "User", HeaderText = "Usuario", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PaymentMethod", HeaderText = "Método Pago", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            Controls.Add(grid);
            Controls.Add(resumenPanel);
            Controls.Add(topPanel);

            btnBuscar.Click += (s, e) => Cargar();
            btnExportar.Click += (s, e) => ExportarAExcel();
            Load += (s, e) => Cargar();

            // Agregar tooltips
            var toolTip = new ToolTip();
            toolTip.SetToolTip(dpDesde, "Fecha de inicio del reporte. Se incluyen ventas desde las 00:00 de este día.");
            toolTip.SetToolTip(dpHasta, "Fecha de fin del reporte. Se incluyen ventas hasta las 23:59 de este día.");
            toolTip.SetToolTip(btnBuscar, "Buscar ventas en el rango de fechas seleccionado.");
            toolTip.SetToolTip(btnExportar, "Exportar los resultados actuales a un archivo CSV para Excel.");
        }

        private void Cargar()
        {
            var desde = dpDesde.Value.Date;
            var hasta = dpHasta.Value.Date.AddDays(1).AddSeconds(-1);
            var ventas = SaleRepository.GetReporteDtoByFecha(desde, hasta);
            grid.DataSource = ventas;

            // Actualizar resumen
            lblCantidad.Text = $"Cantidad de ventas: {ventas.Count}";
            lblTotal.Text = $"Total: {ventas.Sum(v => v.Total):C2}";
        }

        private void ExportarAExcel()
        {
            var ventas = grid.DataSource as System.Collections.Generic.List<DTOs.SaleListDto>;
            if (ventas == null || ventas.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }
            using (var sfd = new SaveFileDialog { Filter = "Archivos CSV (*.csv)|*.csv", FileName = "reporte_ventas.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                        {
                            // Escribir encabezados
                            sw.WriteLine("N° Venta;Fecha;Hora;Usuario;Método Pago;Total");
                            
                            // Escribir filas - usar punto y coma como separador y formato adecuado
                            foreach (var venta in ventas)
                            {
                                var fecha = venta.DateTime.ToString("dd/MM/yyyy");
                                var hora = venta.DateTime.ToString("HH:mm");
                                var total = venta.Total.ToString("F2"); // Formato numérico con 2 decimales
                                
                                sw.WriteLine($"{venta.Id};{fecha};{hora};{venta.User};{venta.PaymentMethod};{total}");
                            }
                        }
                        MessageBox.Show("Exportación exitosa.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar: " + ex.Message);
                    }
                }
            }
        }
    }
}
