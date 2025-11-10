using Minimarket.Data;
using Minimarket.DTOs;
using Models;

namespace Minimarket.UI
{
    public class SalesListForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Button btnVerDetalle = new Button { Text = "Ver Detalle", AutoSize = true };
        private Button btnRefrescar = new Button { Text = "Refrescar", AutoSize = true };

        public SalesListForm()
        {
            Text = "Listado de Ventas";
            Width = 900; Height = 500;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Panel superior con botones
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            btnRefrescar.Margin = new Padding(0, 3, 5, 3);
            btnVerDetalle.Margin = new Padding(5, 3, 0, 3);
            topPanel.Controls.Add(btnRefrescar);
            topPanel.Controls.Add(btnVerDetalle);

            // Configurar grid con columnas en español
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "N° Venta", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Fecha/Hora", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "User", HeaderText = "Usuario", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PaymentMethod", HeaderText = "Método Pago", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            Controls.Add(grid);
            Controls.Add(topPanel);

            Load += (s, e) => Cargar();
            btnRefrescar.Click += (s, e) => Cargar();
            btnVerDetalle.Click += (s, e) => VerDetalle();
            grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) VerDetalle(); };
        }

        private void Cargar()
        {
            grid.DataSource = SaleRepository.GetAllDto();
        }

        private void VerDetalle()
        {
            if (grid.CurrentRow == null) return;
            var dto = grid.CurrentRow.DataBoundItem as SaleListDto;
            if (dto == null) return;
            var venta = SaleRepository.GetByIdWithItems(dto.Id);
            if (venta == null) return;
            var f = new SaleDetailForm(venta);
            f.ShowDialog();
        }
    }

    // Formulario para ver detalle de venta
    public class SaleDetailForm : Form
    {
        private Sale venta;
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Label lblInfo = new Label { AutoSize = true, Font = new Font("Arial", 10, FontStyle.Regular) };
        private Button btnAnular = new Button { Text = "Anular Venta", AutoSize = true };

        public SaleDetailForm(Sale venta)
        {
            this.venta = venta;
            Text = $"Detalle de Venta N° {venta.Id}";
            Width = 800; Height = 400;

            // Verificar permisos para anular
            var usuario = Session.CurrentUser;
            bool puedeAnular = usuario.Role.RoleCode == Constants.RoleCodeSupervisor || 
                              usuario.Role.RoleCode == Constants.RoleCodeAdmin;

            // Panel superior con información
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            lblInfo.Text = $"Fecha: {venta.DateTime:dd/MM/yyyy HH:mm}  |  Método de Pago: {venta.PaymentMethod}  |  Total: {venta.Total:C2}";
            topPanel.Controls.Add(lblInfo);
            
            // Botón anular solo si tiene permisos
            if (puedeAnular)
            {
                btnAnular.Margin = new Padding(20, 0, 0, 0);
                topPanel.Controls.Add(btnAnular);
                btnAnular.Click += (s, e) => AnularVenta();
            }

            // Configurar grid
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Code", HeaderText = "Código", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Producto" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Qty", HeaderText = "Cantidad", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Precio Unit.", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", HeaderText = "Subtotal", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            Controls.Add(grid);
            Controls.Add(topPanel);

            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            grid.DataSource = venta.SaleItems.Select(x => new
            {
                x.Product.Code,
                Name = x.Product.Name,
                x.Qty,
                x.UnitPrice,
                x.Subtotal
            }).ToList();
        }
        
        private void AnularVenta()
        {
            // Solicitar motivo de anulación
            var motivo = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese el motivo de la anulación:",
                "Anular Venta",
                "",
                -1, -1);
                
            if (string.IsNullOrWhiteSpace(motivo))
            {
                MessageBox.Show("Debe ingresar un motivo para anular la venta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (MessageBox.Show($"¿Está seguro que desea anular la venta N° {venta.Id}?\n\nEsta acción no se puede deshacer.",
                "Confirmar Anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SaleRepository.AnularVenta(venta.Id, motivo.Trim());
                    MessageBox.Show("Venta anulada correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al anular venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
