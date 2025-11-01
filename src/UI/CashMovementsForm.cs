using Models;

namespace Minimarket.UI
{
    public class CashMovementsForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private ComboBox cmbTipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown nudMonto = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
        private TextBox txtConcepto = new TextBox { PlaceholderText = "Concepto" };
        private TextBox txtUsuario = new TextBox { PlaceholderText = "ID Usuario" };
        private TextBox txtVenta = new TextBox { PlaceholderText = "ID Venta (opcional)" };
        private Button btnAgregar = new Button { Text = "Registrar Movimiento" };

        public CashMovementsForm()
        {
            Text = "Movimientos de Caja";
            Width = 900; Height = 500;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            cmbTipo.Items.AddRange(new object[] { "IN", "OUT" });
            cmbTipo.SelectedIndex = 0;
            top.Controls.AddRange(new Control[] { new Label { Text = "Tipo:" }, cmbTipo, new Label { Text = "Monto:" }, nudMonto, new Label { Text = "Concepto:" }, txtConcepto, new Label { Text = "ID Usuario:" }, txtUsuario, new Label { Text = "ID Venta:" }, txtVenta, btnAgregar });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Registrar();
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();
            grid.DataSource = db.MovimientosCaja.OrderByDescending(x => x.DateTime).ToList();
        }

        private void Registrar()
        {
            var tipo = cmbTipo.SelectedItem.ToString();
            decimal monto = nudMonto.Value;
            var concepto = txtConcepto.Text.Trim();
            if (string.IsNullOrWhiteSpace(concepto)) { MessageBox.Show("Ingrese un concepto"); return; }
            if (!int.TryParse(txtUsuario.Text, out int userId)) { MessageBox.Show("ID Usuario inválido"); return; }
            int? ventaId = null;
            if (int.TryParse(txtVenta.Text, out int vId)) ventaId = vId;
            using var db = new MinimarketContext();
            var mov = new CashMovement { DateTime = DateTime.Now, Type = tipo, Amount = monto, Concept = concepto, UserId = userId, SaleId = ventaId };
            db.MovimientosCaja.Add(mov);
            db.SaveChanges();
            Cargar();
            nudMonto.Value = 0;
            txtConcepto.Clear();
            txtUsuario.Clear();
            txtVenta.Clear();
        }
    }
}
