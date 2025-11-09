using Models;

namespace Minimarket.UI
{
    public class InventoryMovementsForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private ComboBox cmbProducto = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbTipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown nudCantidad = new NumericUpDown { Minimum = 1, Maximum = 10000 };
        private TextBox txtMotivo = new TextBox { PlaceholderText = "Motivo" };
        private Button btnAgregar = new Button { Text = "Registrar Movimiento" };

        public InventoryMovementsForm()
        {
            Text = "Movimientos de Inventario";
            Width = 900; Height = 500;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            cmbTipo.Items.AddRange(new object[] { "IN", "OUT", "ADJ" });
            cmbTipo.SelectedIndex = 0;
            top.Controls.AddRange(new Control[] { new Label { Text = "Producto:" }, cmbProducto, new Label { Text = "Tipo:" }, cmbTipo, new Label { Text = "Cantidad:" }, nudCantidad, new Label { Text = "Motivo:" }, txtMotivo, btnAgregar });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Registrar();
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();
            cmbProducto.DataSource = db.Productos.ToList();
            cmbProducto.DisplayMember = "Name";
            cmbProducto.ValueMember = "Id";
            grid.DataSource = db.MovimientosInventario.OrderByDescending(x => x.DateTime).ToList();
        }

        private void Registrar()
        {
            if (cmbProducto.SelectedItem == null) { MessageBox.Show("Seleccione un producto"); return; }
            var producto = (Product)cmbProducto.SelectedItem;
            var tipo = cmbTipo.SelectedItem.ToString();
            int cantidad = (int)nudCantidad.Value;
            var motivo = txtMotivo.Text.Trim();
            if (string.IsNullOrWhiteSpace(motivo)) { MessageBox.Show("Ingrese un motivo"); return; }
            using var db = new MinimarketContext();
            var mov = new InventoryMovement { ProductId = producto.Id, DateTime = DateTime.Now, Type = tipo, Qty = cantidad, Reason = motivo };
            db.MovimientosInventario.Add(mov);
            // Actualizar stock
            var prod = db.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (prod != null)
            {
                if (tipo == "IN") prod.Stock += cantidad;
                else if (tipo == "OUT") prod.Stock -= cantidad;
                else if (tipo == "ADJ") prod.Stock = cantidad;
                db.SaveChanges();
            }
            db.SaveChanges();
            Cargar();
            nudCantidad.Value = 1;
            txtMotivo.Clear();
        }
    }
}
