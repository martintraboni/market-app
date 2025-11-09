using Minimarket.Data;
using Minimarket.DTOs;
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
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }


            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 8,
                RowCount = 2,
                Padding = new Padding(8),
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Proveedor
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Combo Proveedor
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Fecha
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); // Fecha
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Doc
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); // Doc
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Botón Agregar
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Botón Detalle

            top.Controls.Add(new Label { Text = "Proveedor:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 0, 0);
            top.Controls.Add(cmbProveedor, 1, 0);
            top.Controls.Add(new Label { Text = "Fecha:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 2, 0);
            top.Controls.Add(dtFecha, 3, 0);
            top.Controls.Add(new Label { Text = "N° Doc usuario:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 4, 0);
            top.Controls.Add(txtDoc, 5, 0);
            top.Controls.Add(btnAgregar, 6, 0);
            top.Controls.Add(btnVerDetalle, 7, 0);


            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Proveedor" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Date", HeaderText = "Fecha" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NroDoc", HeaderText = "Nro. Doc" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total" });

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
            grid.DataSource = PurchaseRepository.GetAllDto();
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
            var dto = grid.CurrentRow.DataBoundItem as PurchaseListDto;
            if (dto == null) return;
            var f = new PurchaseDetailForm(dto.Id);
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
            Width = 900; Height = 350;

            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 8,
                RowCount = 1,
                Padding = new Padding(8),
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Producto
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Combo Producto
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Cantidad
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Proveedor" });
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Costo
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15)); // Costo
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Botón Agregar
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Botón Eliminar

            top.Controls.Add(new Label { Text = "Producto:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 0, 0);
            top.Controls.Add(cmbProducto, 1, 0);
            top.Controls.Add(new Label { Text = "Cantidad:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 2, 0);
            top.Controls.Add(nudCantidad, 3, 0);
            top.Controls.Add(new Label { Text = "Costo:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 4, 0);
            top.Controls.Add(nudCosto, 5, 0);
            top.Controls.Add(btnAgregar, 6, 0);
            top.Controls.Add(btnEliminar, 7, 0);

            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Producto" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Qty", HeaderText = "Cantidad" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cost", HeaderText = "Costo unitario" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", HeaderText = "Subtotal" });

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
            // Usar un DTO para mostrar los datos relevantes en la grilla
            var items = db.DetalleCompras
                .Where(x => x.PurchaseId == compraId)
                .Select(x => new
                {
                    x.Id,
                    ProductName = x.Product.Name,
                    x.Qty,
                    x.Cost,
                    x.Subtotal
                })
                .ToList();
            grid.DataSource = items;
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
            // Actualizar stock y costo
            var prod = db.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (prod != null)
            {
                prod.Stock += cantidad;
                prod.Cost = costo; // Actualizar el costo del producto con el último costo de compra
                db.SaveChanges();
            }
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
            var row = grid.CurrentRow.DataBoundItem;
            var idProp = row.GetType().GetProperty("Id");
            if (idProp == null) return;
            int itemId = (int)idProp.GetValue(row);
            using var db = new MinimarketContext();
            var it = db.DetalleCompras.FirstOrDefault(x => x.Id == itemId);
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
