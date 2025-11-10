using Models;

namespace Minimarket.UI
{
    public class InventoryMovementsForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private ComboBox cmbProducto = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
    private ComboBox cmbTipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300, DropDownWidth = 300 };
        private NumericUpDown nudCantidad = new NumericUpDown { Minimum = 1, Maximum = 10000 };
        private TextBox txtMotivo = new TextBox { PlaceholderText = "Motivo" };
        private Button btnAgregar = new Button { Text = "Registrar Movimiento" };

        public InventoryMovementsForm()
        {
            Text = "Movimientos de Inventario";
            Width = 900; Height = 500;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Verificar permisos: ajustes manuales solo para Supervisor y Admin
            var usuario = Session.CurrentUser;
            bool puedeAjustarStock = usuario.Role.RoleCode == Constants.RoleCodeSupervisor || 
                                     usuario.Role.RoleCode == Constants.RoleCodeAdmin;

            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 9,
                RowCount = 1,
                Padding = new Padding(8),
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Producto
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Combo Producto
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Tipo
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Combo Tipo
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Cantidad
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10)); // Cantidad
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Motivo
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Motivo
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Botón

            cmbTipo.Items.AddRange(new object[] { Constants.InventoryMovementTypeIngreso, Constants.InventoryMovementTypeEgreso, Constants.InventoryMovementTypeAjuste });
            cmbTipo.SelectedIndex = 0;
            top.Controls.Add(new Label { Text = "Producto:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 0, 0);
            top.Controls.Add(cmbProducto, 1, 0);
            top.Controls.Add(new Label { Text = "Tipo:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 2, 0);
            top.Controls.Add(cmbTipo, 3, 0);
            top.Controls.Add(new Label { Text = "Cantidad:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 4, 0);
            top.Controls.Add(nudCantidad, 5, 0);
            top.Controls.Add(new Label { Text = "Motivo:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 6, 0);
            top.Controls.Add(txtMotivo, 7, 0);
            top.Controls.Add(btnAgregar, 8, 0);

            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Product", HeaderText = "Producto" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "Tipo" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Qty", HeaderText = "Cantidad" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Reason", HeaderText = "Motivo" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "Fecha/Hora" });

            Controls.Add(grid);
            Controls.Add(top);
            
            // Deshabilitar controles de ajuste manual si no tiene permisos
            if (!puedeAjustarStock)
            {
                btnAgregar.Enabled = false;
                btnAgregar.Text = "Sin permisos para ajustes";
                cmbProducto.Enabled = false;
                cmbTipo.Enabled = false;
                nudCantidad.Enabled = false;
                txtMotivo.Enabled = false;
            }

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Registrar();
        }

        private void Cargar()
        {
            using var db = new MinimarketContext();
            cmbProducto.DataSource = db.Productos.ToList();
            cmbProducto.DisplayMember = "Name";
            cmbProducto.ValueMember = "Id";
            grid.DataSource = Minimarket.Data.InventoryMovementRepository.GetAllDto();
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
            var prod = db.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (prod == null) { MessageBox.Show("Producto no encontrado"); return; }
            
            // Validar stock según tipo de movimiento
            if (tipo == Constants.InventoryMovementTypeEgreso)
            {
                if (prod.Stock < cantidad)
                {
                    MessageBox.Show($"Stock insuficiente. Stock actual: {prod.Stock}, Cantidad solicitada: {cantidad}");
                    return;
                }
            }
            else if (tipo == Constants.InventoryMovementTypeAjuste)
            {
                if (cantidad < 0)
                {
                    MessageBox.Show("El ajuste de stock no puede ser un valor negativo");
                    return;
                }
            }
            
            var mov = new InventoryMovement { ProductId = producto.Id, DateTime = DateTime.Now, Type = tipo, Qty = cantidad, Reason = motivo };
            db.MovimientosInventario.Add(mov);
            
            // Actualizar stock
            if (tipo == Constants.InventoryMovementTypeIngreso) 
                prod.Stock += cantidad;
            else if (tipo == Constants.InventoryMovementTypeEgreso) 
                prod.Stock -= cantidad;
            else if (tipo == Constants.InventoryMovementTypeAjuste) 
                prod.Stock = cantidad;
            
            db.SaveChanges();
            
            // Registrar ajuste manual en auditoría
            if (tipo == Constants.InventoryMovementTypeAjuste)
            {
                db.Auditoria.Add(new AuditLog
                {
                    UserId = Session.CurrentUser?.Id ?? 1,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventStockAdjustment,
                    Details = $"Producto: {prod.Name} - Nuevo stock: {prod.Stock} - Motivo: {motivo}"
                });
                db.SaveChanges();
            }
            
            MessageBox.Show("Movimiento registrado correctamente");
            Cargar();
            nudCantidad.Value = 1;
            txtMotivo.Clear();
        }
    }
}
