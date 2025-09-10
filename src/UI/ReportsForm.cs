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
        private DataGridView grid = new DataGridView{ Dock = DockStyle.Fill, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill };

        public ReportsForm()
        {
            Text = "Reportes de Ventas";
            Width = 900; Height = 600;

            var top = new FlowLayoutPanel{ Dock = DockStyle.Top, AutoSize=true};
            top.Controls.AddRange(new Control[]{ new Label{ Text="Desde"}, dpDesde, new Label{ Text="Hasta"}, dpHasta, btnBuscar });

            Controls.Add(grid);
            Controls.Add(top);

            btnBuscar.Click += (s,e)=> Cargar();
            Load += (s,e)=> Cargar();
        }

        private void Cargar()
        {
            var dt = SaleRepository.ReporteVentasPorFecha(dpDesde.Value.Date, dpHasta.Value.Date.AddDays(1).AddTicks(-1));
            grid.DataSource = dt;
        }
    }
}
