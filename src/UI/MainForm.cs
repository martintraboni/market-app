namespace Minimarket.UI
{
    public class MainForm : Form
    {
        private MenuStrip menu = new MenuStrip();
        private Label lblUsuario = new Label { Left = 200, Top = 5, AutoSize = true };
        public MainForm(Models.User usuario)
        {
            Text = $"Minimarket - Sistema de Gestión | Usuario: {usuario.FullName}";
            Width = 1000;
            Height = 700;

            var mArchivo = new ToolStripMenuItem("Archivo");
            var mSalir = new ToolStripMenuItem("Salir", null, (s, e) => Close());
            mArchivo.DropDownItems.Add(mSalir);

            var mGestion = new ToolStripMenuItem("Gestión");
            var mProductos = new ToolStripMenuItem("Productos", null, (s, e) => new ProductsForm().ShowDialog());
            var mCategorias = new ToolStripMenuItem("Categorías", null, (s, e) => new CategoriesForm().ShowDialog());
            var mProveedores = new ToolStripMenuItem("Proveedores", null, (s, e) => new SuppliersForm().ShowDialog());
            var mCompras = new ToolStripMenuItem("Compras", null, (s, e) => new PurchasesForm().ShowDialog());
            var mMovInv = new ToolStripMenuItem("Mov. Inventario", null, (s, e) => new InventoryMovementsForm().ShowDialog());
            var mMovCaja = new ToolStripMenuItem("Mov. Caja", null, (s, e) => new CashMovementsForm().ShowDialog());
            var mCierreCaja = new ToolStripMenuItem("Cierre Caja", null, (s, e) => new CashCloseForm().ShowDialog());
            var mVentas = new ToolStripMenuItem("Ventas", null, (s, e) => new SalesForm().ShowDialog());
            var mReportes = new ToolStripMenuItem("Reportes", null, (s, e) => new ReportsForm().ShowDialog());
            mGestion.DropDownItems.AddRange(new ToolStripItem[] { mProductos, mCategorias, mProveedores, mCompras, mMovInv, mMovCaja, mCierreCaja, mVentas, mReportes });

            menu.Items.AddRange(new ToolStripItem[] { mArchivo, mGestion });
            MainMenuStrip = menu;
            Controls.Add(menu);

            lblUsuario.Text = $"Bienvenido, {usuario.FullName} ({usuario.Role})";
            lblUsuario.Top = menu.Height + 5;
            lblUsuario.Left = Width - 250;
            Controls.Add(lblUsuario);
        }
    }
}
