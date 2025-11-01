using Models;

namespace Minimarket.UI
{
    public class CashCloseForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private DateTimePicker dtFecha = new DateTimePicker { Value = DateTime.Today };
        private TextBox txtUsuario = new TextBox { PlaceholderText = "ID Usuario" };
        private NumericUpDown nudEfectivo = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
        private NumericUpDown nudPOS = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
        private NumericUpDown nudSistema = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
        private TextBox txtNotas = new TextBox { PlaceholderText = "Notas", Width = 200 };
        private Button btnCerrar = new Button { Text = "Registrar Cierre" };

        public CashCloseForm()
        {
            Text = "Cierre / Arqueo de Caja";
            Width = 900; Height = 500;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            top.Controls.AddRange(new Control[] {
                new Label { Text = "Fecha:" }, dtFecha,
                new Label { Text = "ID Usuario:" }, txtUsuario,
                new Label { Text = "Efectivo contado:" }, nudEfectivo,
                new Label { Text = "Total POS:" }, nudPOS,
                new Label { Text = "Total Sistema:" }, nudSistema,
                new Label { Text = "Notas:" }, txtNotas,
                btnCerrar
            });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnCerrar.Click += (s, e) => Registrar();
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();
            grid.DataSource = db.CierresCaja.OrderByDescending(x => x.Date).ToList();
        }

        private void Registrar()
        {
            if (!int.TryParse(txtUsuario.Text, out int userId)) { MessageBox.Show("ID Usuario inválido"); return; }
            decimal efectivo = nudEfectivo.Value;
            decimal pos = nudPOS.Value;
            decimal sistema = nudSistema.Value;
            decimal diferencia = (efectivo + pos) - sistema;
            var notas = txtNotas.Text.Trim();
            using var db = new MinimarketContext();
            var cierre = new CashClose
            {
                Date = dtFecha.Value.Date,
                UserId = userId,
                CashInHand = efectivo,
                PosTotal = pos,
                SystemTotal = sistema,
                Difference = diferencia,
                Notes = notas
            };
            db.CierresCaja.Add(cierre);
            db.SaveChanges();
            Cargar();
            nudEfectivo.Value = 0;
            nudPOS.Value = 0;
            nudSistema.Value = 0;
            txtNotas.Clear();
        }
    }
}
