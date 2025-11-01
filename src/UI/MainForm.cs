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
            var mVentas = new ToolStripMenuItem("Ventas", null, (s, e) => new SalesForm().ShowDialog());
            var mReportes = new ToolStripMenuItem("Reportes", null, (s, e) => new ReportsForm().ShowDialog());
            mGestion.DropDownItems.AddRange(new ToolStripItem[] { mProductos, mVentas, mReportes });

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
