using Models;

namespace Minimarket.UI
{
    public class PurchasesForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private ComboBox cmbProveedor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtDoc = new TextBox { PlaceholderText = "Nro. Documento" };
        private DateTimePicker dtFecha = new DateTimePicker { Value = DateTime.Today };
        private Button btnAgregar = new Button { Text = "Nueva Compra" };
        private Button btnVerDetalle = new Button { Text = "Ver Detalle" };

        public PurchasesForm()
        {
            Text = "Compras";
            Width = 900; Height = 500;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            top.Controls.AddRange(new Control[] { new Label { Text = "Proveedor:" }, cmbProveedor, new Label { Text = "Fecha:" }, dtFecha, new Label { Text = "Nro. Doc:" }, txtDoc, btnAgregar, btnVerDetalle });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => NuevaCompra();
            btnVerDetalle.Click += (s, e) => VerDetalle();
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();
            cmbProveedor.DataSource = db.Proveedores.ToList();
            cmbProveedor.DisplayMember = "Name";
            cmbProveedor.ValueMember = "Id";
            grid.DataSource = db.Compras.ToList();
        }

        private void NuevaCompra()
        {
            if (cmbProveedor.SelectedItem == null) { MessageBox.Show("Seleccione un proveedor"); return; }
            var proveedor = (Supplier)cmbProveedor.SelectedItem;
            var fecha = dtFecha.Value.Date;
            var doc = txtDoc.Text.Trim();
            if (string.IsNullOrWhiteSpace(doc)) { MessageBox.Show("Ingrese nro. de documento"); return; }
            using var db = new MinimarketContext();
            var compra = new Purchase { SupplierId = proveedor.Id, Date = fecha, DocNumber = doc, Total = 0 };
            db.Compras.Add(compra);
            db.SaveChanges();
            MessageBox.Show("Compra registrada. Ahora agregue los ítems desde el detalle.");
            Cargar();
        }

        private void VerDetalle()
        {
            if (grid.CurrentRow == null) return;
            var compra = (Purchase)grid.CurrentRow.DataBoundItem;
            var f = new PurchaseDetailForm(compra.Id);
            f.ShowDialog();
            Cargar();
        }
    }

    // Formulario para detalle de compra (ítems)
    public class PurchaseDetailForm : Form
    {
        private int compraId;
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private ComboBox cmbProducto = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown nudCantidad = new NumericUpDown { Minimum = 1, Maximum = 10000 };
        private NumericUpDown nudCosto = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
        private Button btnAgregar = new Button { Text = "Agregar Ítem" };
        private Button btnEliminar = new Button { Text = "Eliminar Ítem" };

        public PurchaseDetailForm(int compraId)
        {
            this.compraId = compraId;
            Text = "Detalle de Compra";
            Width = 700; Height = 400;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            top.Controls.AddRange(new Control[] { new Label { Text = "Producto:" }, cmbProducto, new Label { Text = "Cantidad:" }, nudCantidad, new Label { Text = "Costo:" }, nudCosto, btnAgregar, btnEliminar });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Agregar();
            btnEliminar.Click += (s, e) => Eliminar();
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();
            cmbProducto.DataSource = db.Productos.ToList();
            cmbProducto.DisplayMember = "Name";
            cmbProducto.ValueMember = "Id";
            grid.DataSource = db.DetalleCompras.Where(x => x.PurchaseId == compraId).ToList();
        }

        private void Agregar()
        {
            if (cmbProducto.SelectedItem == null) { MessageBox.Show("Seleccione un producto"); return; }
            var producto = (Product)cmbProducto.SelectedItem;
            int cantidad = (int)nudCantidad.Value;
            decimal costo = nudCosto.Value;
            if (cantidad <= 0 || costo < 0) { MessageBox.Show("Cantidad y costo válidos"); return; }
            using var db = new MinimarketContext();
            db.DetalleCompras.Add(new PurchaseItem { PurchaseId = compraId, ProductId = producto.Id, Qty = cantidad, Cost = costo, Subtotal = cantidad * costo });
            db.SaveChanges();
            // Actualizar stock
            var prod = db.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (prod != null) { prod.Stock += cantidad; db.SaveChanges(); }
            // Actualizar total de compra
            var compra = db.Compras.FirstOrDefault(c => c.Id == compraId);
            if (compra != null)
            {
                compra.Total = db.DetalleCompras.Where(x => x.PurchaseId == compraId).Sum(x => x.Subtotal);
                db.SaveChanges();
            }
            Cargar();
        }

        private void Eliminar()
        {
            if (grid.CurrentRow == null) return;
            var item = (PurchaseItem)grid.CurrentRow.DataBoundItem;
            using var db = new MinimarketContext();
            var it = db.DetalleCompras.FirstOrDefault(x => x.Id == item.Id);
            if (it != null)
            {
                // Descontar stock
                var prod = db.Productos.FirstOrDefault(p => p.Id == it.ProductId);
                if (prod != null) { prod.Stock -= it.Qty; }
                db.DetalleCompras.Remove(it);
                db.SaveChanges();
                // Actualizar total de compra
                var compra = db.Compras.FirstOrDefault(c => c.Id == compraId);
                if (compra != null)
                {
                    compra.Total = db.DetalleCompras.Where(x => x.PurchaseId == compraId).Sum(x => x.Subtotal);
                    db.SaveChanges();
                }
                Cargar();
            }
        }
    }
}
