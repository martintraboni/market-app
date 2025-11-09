using System;
using System.Linq;
using System.Windows.Forms;
using Minimarket.Data;

namespace Minimarket.UI
{
    public class AuditLogsForm : Form
    {
        private DateTimePicker dpDesde = new DateTimePicker { Value = DateTime.Today.AddDays(-30), Width = 120 };
        private DateTimePicker dpHasta = new DateTimePicker { Value = DateTime.Today, Width = 120 };
        private Button btnBuscar = new Button { Text = "Buscar", AutoSize = true };
        private Button btnMostrarTodo = new Button { Text = "Mostrar Todo", AutoSize = true };
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Label lblTotal = new Label { AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold) };

        public AuditLogsForm()
        {
            Text = "Auditoría del Sistema";
            Width = 1100; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Panel superior con controles de filtrado
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
            btnMostrarTodo.Margin = new Padding(5, 3, 15, 3);
            topPanel.Controls.Add(btnMostrarTodo);

            // Espaciador
            topPanel.Controls.Add(new Panel { Width = 20, Height = 1 });
            topPanel.Controls.Add(lblTotal);

            // Panel de información
            var infoPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = System.Drawing.Color.LightBlue
            };
            var lblInfo = new Label 
            { 
                Text = "ℹ️ Esta vista muestra todas las operaciones críticas realizadas en el sistema (creación, modificación, eliminación de usuarios).",
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(1050, 0)
            };
            infoPanel.Controls.Add(lblInfo);

            // Configurar grid con columnas en español
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Fecha/Hora", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "User", HeaderText = "Usuario", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Event", HeaderText = "Evento", Width = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Details", HeaderText = "Detalles", Width = 400 });

            Controls.Add(grid);
            Controls.Add(infoPanel);
            Controls.Add(topPanel);

            btnBuscar.Click += (s, e) => CargarPorFecha();
            btnMostrarTodo.Click += (s, e) => CargarTodo();
            Load += (s, e) => CargarPorFecha();

            // Agregar tooltips
            var toolTip = new ToolTip();
            toolTip.SetToolTip(dpDesde, "Fecha de inicio del rango de auditoría.");
            toolTip.SetToolTip(dpHasta, "Fecha de fin del rango de auditoría.");
            toolTip.SetToolTip(btnBuscar, "Buscar registros de auditoría en el rango de fechas seleccionado.");
            toolTip.SetToolTip(btnMostrarTodo, "Mostrar todos los registros de auditoría sin filtro de fecha.");
        }

        private void CargarPorFecha()
        {
            var desde = dpDesde.Value.Date;
            var hasta = dpHasta.Value.Date.AddDays(1).AddSeconds(-1);
            var logs = AuditLogRepository.GetByDateRange(desde, hasta);
            grid.DataSource = logs;
            lblTotal.Text = $"Total de registros: {logs.Count}";
        }

        private void CargarTodo()
        {
            var logs = AuditLogRepository.GetAllDto();
            grid.DataSource = logs;
            lblTotal.Text = $"Total de registros: {logs.Count}";
        }
    }
}
