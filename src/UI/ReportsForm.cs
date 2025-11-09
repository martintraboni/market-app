using System;
using System.Windows.Forms;
using Minimarket.Data;

namespace Minimarket.UI
{
    public class ReportsForm : Form
    {
    private DateTimePicker dpDesde = new DateTimePicker{ Value = DateTime.Today.AddDays(-7)};
    private DateTimePicker dpHasta = new DateTimePicker{ Value = DateTime.Today};
    private Button btnBuscar = new Button{ Text="Buscar"};
    private Button btnExportar = new Button{ Text="Exportar a Excel"};
    private DataGridView grid = new DataGridView{ Dock = DockStyle.Fill, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill };

        public ReportsForm()
        {
            Text = "Reportes de Ventas";
            Width = 900; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            var top = new FlowLayoutPanel{ Dock = DockStyle.Top, AutoSize=true};
            top.Controls.AddRange(new Control[]{ new Label{ Text="Desde"}, dpDesde, new Label{ Text="Hasta"}, dpHasta, btnBuscar, btnExportar });

            Controls.Add(grid);
            Controls.Add(top);

            btnBuscar.Click += (s,e)=> Cargar();
            btnExportar.Click += (s,e)=> ExportarAExcel();
            Load += (s,e)=> Cargar();
        }

        private void Cargar()
        {
            var dt = SaleRepository.ReporteVentasPorFecha(dpDesde.Value.Date, dpHasta.Value.Date.AddDays(1).AddTicks(-1));
            grid.DataSource = dt;
        }

        private void ExportarAExcel()
        {
            if (grid.DataSource == null)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }
            using (var sfd = new SaveFileDialog { Filter = "Archivos Excel (*.csv)|*.csv", FileName = "reporte_ventas.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                        {
                            // Escribir encabezados
                            for (int i = 0; i < grid.Columns.Count; i++)
                            {
                                sw.Write(grid.Columns[i].HeaderText);
                                if (i < grid.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();
                            // Escribir filas
                            foreach (DataGridViewRow row in grid.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    for (int i = 0; i < grid.Columns.Count; i++)
                                    {
                                        var val = row.Cells[i].Value?.ToString()?.Replace("\"", "\"") ?? "";
                                        if (val.Contains(",") || val.Contains("\n"))
                                            val = $"\"{val}\"";
                                        sw.Write(val);
                                        if (i < grid.Columns.Count - 1) sw.Write(",");
                                    }
                                    sw.WriteLine();
                                }
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
