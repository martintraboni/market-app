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

            // Menú Reportes
            var mReportes = new ToolStripMenuItem("Reportes");
            var mReporteVentas = new ToolStripMenuItem("Reporte de Ventas", null, (s, e) => new ReportsForm().ShowDialog());
            mReportes.DropDownItems.Add(mReporteVentas);

            // Menú Gestión Admin (solo visible para administradores)
            ToolStripMenuItem? mAdmin = null;
            if (usuario.Role.RoleCode == Constants.RoleCodeAdmin)
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
        }
    }
}
