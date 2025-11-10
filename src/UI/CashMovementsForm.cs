using Minimarket.Data;
using Minimarket.DTOs;
using Models;

namespace Minimarket.UI
{
    public class CashMovementsForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private ComboBox cmbTipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 100 };
        private NumericUpDown nudMonto = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Width = 100 };
        private TextBox txtConcepto = new TextBox { PlaceholderText = "Concepto", Width = 300 };
        private Button btnAgregar = new Button { Text = "Registrar Movimiento", AutoSize = true };
        private Label lblSaldo = new Label { AutoSize = true, Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.Green };

        public CashMovementsForm()
        {
            Text = "Movimientos de Caja";
            Width = 1200; Height = 600;
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

            cmbTipo.Items.AddRange(new object[] { Constants.CashMovementTypeIn, Constants.CashMovementTypeOut });
            cmbTipo.SelectedIndex = 0;

            topPanel.Controls.Add(new Label { Text = "Tipo:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) });
            topPanel.Controls.Add(cmbTipo);
            topPanel.Controls.Add(new Label { Text = "Monto:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            topPanel.Controls.Add(nudMonto);
            topPanel.Controls.Add(new Label { Text = "Concepto:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            topPanel.Controls.Add(txtConcepto);
            btnAgregar.Margin = new Padding(15, 3, 0, 3);
            topPanel.Controls.Add(btnAgregar);
            
            // Espaciador
            topPanel.Controls.Add(new Panel { Width = 30, Height = 1 });
            
            // Label de saldo
            lblSaldo.Margin = new Padding(15, 6, 0, 0);
            topPanel.Controls.Add(lblSaldo);

            // Configurar grid con columnas en español
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "Tipo", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Amount", HeaderText = "Monto", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Concept", HeaderText = "Concepto" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "User", HeaderText = "Usuario", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Fecha/Hora", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });

            Controls.Add(grid);
            Controls.Add(topPanel);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Registrar();
        }

        private void Cargar()
        {
            grid.DataSource = CashMovementRepository.GetAllDto();
            ActualizarSaldo();
        }

        private void ActualizarSaldo()
        {
            var saldo = CashMovementRepository.GetSaldoCaja();
            lblSaldo.Text = $"Saldo en Caja: {saldo:C2}";
            lblSaldo.ForeColor = saldo >= 0 ? Color.Green : Color.Red;
        }

        private void Registrar()
        {
            var tipo = cmbTipo.SelectedItem.ToString();
            decimal monto = nudMonto.Value;
            var concepto = txtConcepto.Text.Trim();
            
            if (monto <= 0) 
            { 
                MessageBox.Show("El monto debe ser mayor a cero"); 
                return; 
            }
            
            if (string.IsNullOrWhiteSpace(concepto)) 
            { 
                MessageBox.Show("Ingrese un concepto"); 
                return; 
            }
            
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("No hay usuario logueado");
                return;
            }
            
            var mov = new CashMovement 
            { 
                DateTime = DateTime.Now, 
                Type = tipo, 
                Amount = monto, 
                Concept = concepto, 
                UserId = Session.CurrentUser.Id, 
                SaleId = null
            };
            
            CashMovementRepository.Create(mov);
            MessageBox.Show("Movimiento registrado correctamente");
            
            Cargar();
            nudMonto.Value = 0;
            txtConcepto.Clear();
        }
    }
}
