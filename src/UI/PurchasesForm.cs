using Minimarket.Data;
using Minimarket.DTOs;
using Models;

namespace Minimarket.UI
{
    public class PurchasesForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private TextBox txtBuscar = new TextBox { PlaceholderText = "Buscar por proveedor...", Width = 250 };
        private Button btnAgregar = new Button { Text = "Nueva Compra", AutoSize = true };
        private Button btnVerDetalle = new Button { Text = "Ver Detalle", AutoSize = true };

        public PurchasesForm()
        {
            Text = "Compras";
            Width = 900; Height = 500;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Panel superior con controles
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            topPanel.Controls.Add(new Label { Text = "Buscar:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 5, 0) });
            topPanel.Controls.Add(txtBuscar);

            // Espaciador para empujar botones a la derecha
            var spacer = new Panel { Width = 350, Height = 1 };
            topPanel.Controls.Add(spacer);

            btnAgregar.Margin = new Padding(5, 3, 5, 3);
            btnVerDetalle.Margin = new Padding(0, 3, 5, 3);
            topPanel.Controls.Add(btnAgregar);
            topPanel.Controls.Add(btnVerDetalle);


            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Proveedor" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Date", HeaderText = "Fecha" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NroDoc", HeaderText = "Nro. Doc" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            Controls.Add(grid);
            Controls.Add(topPanel);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => NuevaCompra();
            btnVerDetalle.Click += (s, e) => VerDetalle();
            txtBuscar.TextChanged += (s, e) => FiltrarPorProveedor();
        }

        private void FiltrarPorProveedor()
        {
            var busqueda = txtBuscar.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                grid.DataSource = PurchaseRepository.GetAllDto();
            }
            else
            {
                var todas = PurchaseRepository.GetAllDto();
                var filtradas = todas.Where(c => c.SupplierName.ToLower().Contains(busqueda)).ToList();
                grid.DataSource = filtradas;
            }
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();
            grid.DataSource = PurchaseRepository.GetAllDto();
        }

        private void NuevaCompra()
        {
            var f = new PurchaseDetailForm(null, true);
            if (f.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Compra registrada correctamente.");
                txtBuscar.Clear();
                Cargar();
            }
        }

        private void VerDetalle()
        {
            if (grid.CurrentRow == null) return;
            var dto = grid.CurrentRow.DataBoundItem as PurchaseListDto;
            if (dto == null) return;
            var compra = PurchaseRepository.GetByIdWithItems(dto.Id);
            if (compra == null) return;
            var f = new PurchaseDetailForm(compra, false);
            f.ShowDialog();
            Cargar();
        }
    }

    // Formulario para detalle de compra (ítems)
    public class PurchaseDetailForm : Form
    {
        private Purchase compra;
        private bool esNueva;
        private ComboBox cmbProveedor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
        private TextBox txtDoc = new TextBox { PlaceholderText = "Nro. Documento", Width = 150 };
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private ComboBox cmbProducto = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown nudCantidad = new NumericUpDown { Minimum = 1, Maximum = 10000 };
        private NumericUpDown nudCosto = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
        private Button btnAgregar = new Button { Text = "Agregar producto" };
        private Button btnAceptar = new Button { Text = "Aceptar" };
        private Button btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };

        public PurchaseDetailForm(Purchase compra, bool esNueva)
        {
            this.compra = compra;
            this.esNueva = esNueva;
            Text = esNueva ? "Nueva Compra" : "Detalle de Compra";
            Width = 900; Height = 450;

            // Panel superior con proveedor y documento (solo si es nueva)
            var headerPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Visible = esNueva
            };

            headerPanel.Controls.Add(new Label { Text = "Proveedor:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) });
            headerPanel.Controls.Add(cmbProveedor);
            headerPanel.Controls.Add(new Label { Text = "N° Doc:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            headerPanel.Controls.Add(txtDoc);

            // Panel para controles de producto
            var productPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            productPanel.Controls.Add(new Label { Text = "Producto:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) });
            cmbProducto.Width = 200;
            productPanel.Controls.Add(cmbProducto);

            productPanel.Controls.Add(new Label { Text = "Cantidad:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            nudCantidad.Width = 80;
            productPanel.Controls.Add(nudCantidad);

            productPanel.Controls.Add(new Label { Text = "Costo:", AutoSize = true, Margin = new Padding(15, 6, 5, 0) });
            nudCosto.Width = 100;
            productPanel.Controls.Add(nudCosto);

            btnAgregar.Margin = new Padding(15, 3, 5, 3);
            productPanel.Controls.Add(btnAgregar);

            // Panel inferior con botones de acción
            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.RightToLeft
            };

            btnCancelar.Margin = new Padding(0, 3, 5, 3);
            btnAceptar.Margin = new Padding(5, 3, 0, 3);
            bottomPanel.Controls.Add(btnCancelar);
            bottomPanel.Controls.Add(btnAceptar);

            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Producto" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Qty", HeaderText = "Cantidad" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cost", HeaderText = "Costo unitario", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", HeaderText = "Subtotal", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });

            Controls.Add(grid);
            Controls.Add(productPanel);
            Controls.Add(headerPanel);
            Controls.Add(bottomPanel);

            // Si es solo visualización, deshabilitar controles de edición
            if (!esNueva)
            {
                cmbProducto.Enabled = false;
                nudCantidad.Enabled = false;
                nudCosto.Enabled = false;
                btnAgregar.Enabled = false;
                btnAceptar.Enabled = false;
                btnCancelar.Text = "Cerrar";
            }

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Agregar();
            btnAceptar.Click += (s, e) =>
            {
                if (esNueva)
                {
                    // Validar proveedor y documento
                    if (cmbProveedor.SelectedItem == null)
                    {
                        MessageBox.Show("Seleccione un proveedor");
                        return;
                    }
                    var doc = txtDoc.Text.Trim();
                    if (string.IsNullOrWhiteSpace(doc))
                    {
                        MessageBox.Show("Ingrese nro. de documento");
                        return;
                    }

                    // Debug: verificar estado
                    var itemsCount = this.compra?.PurchaseItems?.Count ?? 0;

                    // Validar que haya productos agregados
                    if (this.compra == null || this.compra.PurchaseItems == null || this.compra.PurchaseItems.Count == 0)
                    {
                        MessageBox.Show($"Debe agregar al menos un producto a la compra.");
                        return;
                    }

                    // Actualizar datos del proveedor y documento en el objeto compra
                    var proveedor = (Supplier)cmbProveedor.SelectedItem;
                    this.compra.SupplierId = proveedor.Id;
                    this.compra.Date = DateTime.Now;
                    this.compra.DocNumber = doc;
                }

                var result = MessageBox.Show("¿Está seguro que desea finalizar la compra?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (esNueva)
                    {
                        // Preguntar si se pagó en efectivo
                        var pagoEfectivo = MessageBox.Show("¿Esta compra fue pagada en efectivo?", "Pago en efectivo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        bool registrarEgreso = pagoEfectivo == DialogResult.Yes;

                        PurchaseRepository.CrearCompra(this.compra, registrarEgreso);
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };
            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();

            // Cargar proveedores si es nueva compra
            if (esNueva)
            {
                if (cmbProveedor.DataSource == null)
                {
                    cmbProveedor.DataSource = db.Proveedores.ToList();
                    cmbProveedor.DisplayMember = "Name";
                    cmbProveedor.ValueMember = "Id";
                }
            }

            if (cmbProducto.DataSource == null)
            {
                cmbProducto.DataSource = db.Productos.ToList();
                cmbProducto.DisplayMember = "Name";
                cmbProducto.ValueMember = "Id";
            }

            if (esNueva)
            {
                if (this.compra == null || this.compra.PurchaseItems == null || this.compra.PurchaseItems.Count == 0)
                {
                    // Inicializar lista vacía para mostrar en la grilla
                    grid.DataSource = new List<object>();
                }
                else
                {
                    var items = this.compra.PurchaseItems.Select(x => new
                    {
                        x.Id,
                        ProductName = db.Productos.FirstOrDefault(p => p.Id == x.ProductId)?.Name ?? "",
                        x.Qty,
                        x.Cost,
                        x.Subtotal
                    }).ToList();
                    grid.DataSource = items;
                }
            }
            else
            {
                var items = db.DetalleCompras
                    .Where(x => x.PurchaseId == compra.Id)
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
        }

        private void Agregar()
        {
            if (cmbProducto.SelectedItem == null) { MessageBox.Show("Seleccione un producto"); return; }
            var producto = (Product)cmbProducto.SelectedItem;
            int cantidad = (int)nudCantidad.Value;
            decimal costo = nudCosto.Value;
            if (cantidad <= 0 || costo < 0) { MessageBox.Show("Cantidad y costo válidos"); return; }
            if (esNueva)
            {
                // Crear compra temporal si no existe
                if (this.compra == null)
                {
                    this.compra = new Purchase
                    {
                        PurchaseItems = new List<PurchaseItem>()
                    };
                }

                this.compra.PurchaseItems.Add(new PurchaseItem { ProductId = producto.Id, Qty = cantidad, Cost = costo, Subtotal = cantidad * costo });
                this.compra.Total = this.compra.PurchaseItems.Sum(x => x.Subtotal);
            }
            else
            {
                try
                {
                    PurchaseRepository.AgregarItem(compra.Id, producto.Id, cantidad, costo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al agregar ítem: {ex.Message}");
                }
            }
            Cargar();
        }
    }
}
