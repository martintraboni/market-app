using MiniMarket.Data;
using Models;

namespace Minimarket.UI
{
    public class ProductEditForm : Form
    {
        public Product Producto { get; private set; }
        private bool esNuevo;

        TextBox txtCodigo = new TextBox();
        TextBox txtDescripcion = new TextBox();
        ComboBox cmbCategoria = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        NumericUpDown nudPrecio = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000 };
        NumericUpDown nudStock = new NumericUpDown { Maximum = 1000000 };
        NumericUpDown nudStockMin = new NumericUpDown { Maximum = 1000000 };
        Button btnOk = new Button { Text = "Guardar" };
        Button btnCancel = new Button { Text = "Cancelar" };

        private List<Category> categorias;

        public ProductEditForm(Product p, bool esNuevo)
        {
            this.esNuevo = esNuevo;
            Producto = new Product
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Category = p.Category,
                CategoryId = p.CategoryId,
                Price = p.Price,
                Stock = p.Stock,
                MinStock = p.MinStock,
                IsActive = esNuevo ? true : p.IsActive
            };

            Text = esNuevo ? "Nuevo Producto" : "Editar Producto";
            Width = 400; Height = 400;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, AutoSize = true };
            layout.Controls.Add(new Label { Text = "Código" }, 0, 0); layout.Controls.Add(txtCodigo, 1, 0);
            layout.Controls.Add(new Label { Text = "Descripción" }, 0, 1); layout.Controls.Add(txtDescripcion, 1, 1);
            layout.Controls.Add(new Label { Text = "Categoría" }, 0, 2); layout.Controls.Add(cmbCategoria, 1, 2);
            layout.Controls.Add(new Label { Text = "Precio" }, 0, 3); layout.Controls.Add(nudPrecio, 1, 3);
            layout.Controls.Add(new Label { Text = "Stock" }, 0, 4); layout.Controls.Add(nudStock, 1, 4);
            layout.Controls.Add(new Label { Text = "Stock Mínimo" }, 0, 5); layout.Controls.Add(nudStockMin, 1, 5);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true };
            buttons.Controls.AddRange(new Control[] { btnOk, btnCancel });

            Controls.Add(layout); Controls.Add(buttons);

            // Cargar categorías en el ComboBox
            categorias = CategoryRepository.GetAll();
            cmbCategoria.DataSource = categorias;
            cmbCategoria.DisplayMember = "Name";
            cmbCategoria.ValueMember = "Id";

            // Inicializar campos
            txtCodigo.Text = Producto.Code;
            txtDescripcion.Text = Producto.Name;
            if (Producto.CategoryId.HasValue)
                cmbCategoria.SelectedValue = Producto.CategoryId.Value;
            nudPrecio.Value = Producto.Price;
            nudStock.Value = Producto.Stock;
            nudStockMin.Value = Producto.MinStock;

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtDescripcion.Text))
                {
                    MessageBox.Show("Código y Descripción son obligatorios"); return;
                }
                if (cmbCategoria.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar una categoría"); return;
                }
                if (nudStock.Value < nudStockMin.Value)
                {
                    if (MessageBox.Show("El stock es menor al stock mínimo. ¿Desea continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        return;
                }
                Producto.Code = txtCodigo.Text.Trim();
                Producto.Name = txtDescripcion.Text.Trim();
                // Solo asignar CategoryId, no la entidad Category, para evitar problemas de tracking de EF
                Producto.CategoryId = ((Category)cmbCategoria.SelectedItem).Id;
                // Producto.Category = null; // Opcional: asegurarse de que no se asigne la entidad
                Producto.Price = nudPrecio.Value;
                Producto.Stock = (int)nudStock.Value;
                Producto.MinStock = (int)nudStockMin.Value;
                DialogResult = DialogResult.OK;
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }
    }
}
