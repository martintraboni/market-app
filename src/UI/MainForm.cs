namespace Minimarket.UI
{
    public class MainForm : Form
    {
        private MenuStrip menu = new MenuStrip();
        private Label lblUsuario = new Label { Left = 200, Top = 5, AutoSize = true };
        public MainForm(Models.User usuario)
        {
            Text = $"Minimarket - Sistema de Gestin | Usuario: {usuario.FullName}";
            Width = 1000;
            Height = 700;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            string roleCode = usuario.Role.RoleCode;
            bool isAdmin = roleCode == Constants.RoleCodeAdmin;
            bool isSupervisor = roleCode == Constants.RoleCodeSupervisor;
            bool isUser = roleCode == Constants.RoleCodeUser;

            // Menú Productos
            var mProductos = new ToolStripMenuItem("Productos");
            var mGestionProductos = new ToolStripMenuItem("Gestión de Productos", null, (s, e) => new ProductsForm().ShowDialog());
            var mCategorias = new ToolStripMenuItem("Categorías", null, (s, e) => new CategoriesForm().ShowDialog());
            var mMovInv = new ToolStripMenuItem("Movimientos de Inventario", null, (s, e) => new InventoryMovementsForm().ShowDialog());
            mProductos.DropDownItems.AddRange(new ToolStripItem[] { mGestionProductos, mCategorias, mMovInv });

            // Menú Compras
            var mCompras = new ToolStripMenuItem("Compras");
            var mGestionCompras = new ToolStripMenuItem("Gestión de Compras", null, (s, e) => new PurchasesForm().ShowDialog());
            var mProveedores = new ToolStripMenuItem("Proveedores", null, (s, e) => new SuppliersForm().ShowDialog());
            mCompras.DropDownItems.AddRange(new ToolStripItem[] { mGestionCompras, mProveedores });
            
            // Proveedores solo para Supervisor y Admin
            mProveedores.Enabled = isSupervisor || isAdmin;

            // Menú Ventas
            var mVentas = new ToolStripMenuItem("Ventas");
            var mNuevaVenta = new ToolStripMenuItem("Nueva Venta", null, (s, e) => new SalesForm().ShowDialog());
            var mListadoVentas = new ToolStripMenuItem("Listado de Ventas", null, (s, e) => new SalesListForm().ShowDialog());
            mVentas.DropDownItems.AddRange(new ToolStripItem[] { mNuevaVenta, mListadoVentas });

            // Menú Caja
            var mCaja = new ToolStripMenuItem("Caja");
            var mMovCaja = new ToolStripMenuItem("Movimientos de Caja", null, (s, e) => new CashMovementsForm().ShowDialog());
            var mCierreCaja = new ToolStripMenuItem("Cierre de Caja", null, (s, e) => new CashCloseForm().ShowDialog());
            mCaja.DropDownItems.AddRange(new ToolStripItem[] { mMovCaja, mCierreCaja });
            
            // Movimientos y Cierre de Caja solo para Supervisor y Admin
            mMovCaja.Enabled = isSupervisor || isAdmin;
            mCierreCaja.Enabled = isSupervisor || isAdmin;

            // Menú Reportes
            var mReportes = new ToolStripMenuItem("Reportes");
            var mReporteVentas = new ToolStripMenuItem("Reporte de Ventas", null, (s, e) => new ReportsForm().ShowDialog());
            var mReporteBajoStock = new ToolStripMenuItem("Productos con Bajo Stock", null, (s, e) => new LowStockReportForm().ShowDialog());
            var mReportesAvanzados = new ToolStripMenuItem("Reportes Avanzados", null, (s, e) => new AdvancedReportsForm().ShowDialog());
            mReportes.DropDownItems.AddRange(new ToolStripItem[] { mReporteVentas, mReporteBajoStock, mReportesAvanzados });

            // Menú Gestión Admin (solo visible para administradores)
            ToolStripMenuItem? mAdmin = null;
            if (isAdmin)
            {
                mAdmin = new ToolStripMenuItem("Gestión Admin");
                var mUsuarios = new ToolStripMenuItem("Usuarios", null, (s, e) => new UsersForm().ShowDialog());
                var mAuditoria = new ToolStripMenuItem("Auditoría", null, (s, e) => new AuditLogsForm().ShowDialog());
                mAdmin.DropDownItems.AddRange(new ToolStripItem[] { mUsuarios, mAuditoria });
            }

            // Menú Salir
            var mSalir = new ToolStripMenuItem("Cerrar Sesión", null, (s, e) =>
            {
                if (MessageBox.Show("¿Está seguro que desea cerrar sesión?", "Confirmar", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Close();
                }
            });
            
            // Agregar menús (incluyendo Admin si aplica)
            var menuItems = new System.Collections.Generic.List<ToolStripItem> { mProductos, mCompras, mVentas, mCaja, mReportes };
            if (mAdmin != null)
                menuItems.Add(mAdmin);
            menuItems.Add(mSalir);
            menu.Items.AddRange(menuItems.ToArray());
            MainMenuStrip = menu;
            Controls.Add(menu);

            lblUsuario.Text = $"Bienvenido, {usuario.FullName} ({usuario.Role.RoleDescription})";
            lblUsuario.Top = menu.Height + 5;
            lblUsuario.Left = Width - 250;
            Controls.Add(lblUsuario);
            
            // Panel de alertas de bajo stock
            var panelAlertas = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = System.Drawing.Color.FromArgb(255, 243, 205),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            
            var lblAlerta = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = System.Drawing.Color.DarkOrange
            };
            
            var btnVerBajoStock = new Button
            {
                Text = "Ver Detalles →",
                Dock = DockStyle.Right,
                Width = 120,
                BackColor = System.Drawing.Color.Orange,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVerBajoStock.FlatAppearance.BorderSize = 0;
            btnVerBajoStock.Click += (s, e) => new LowStockReportForm().ShowDialog();
            
            panelAlertas.Controls.Add(lblAlerta);
            panelAlertas.Controls.Add(btnVerBajoStock);
            
            // Verificar productos con bajo stock
            var productosBajoStock = Minimarket.Data.ProductRepository.GetProductosBajoStock();
            if (productosBajoStock.Count > 0)
            {
                lblAlerta.Text = $"⚠️ ALERTA: {productosBajoStock.Count} producto(s) con stock bajo o agotado. Se recomienda realizar un pedido.";
                Controls.Add(panelAlertas);
            }
        }
    }
}
