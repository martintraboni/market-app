using System;
using System.Windows.Forms;
using Minimarket.Models;

namespace Minimarket.UI
{
    public class ProductEditForm : Form
    {
        public Product Producto { get; private set; }
        private bool esNuevo;

        TextBox txtCodigo = new TextBox();
        TextBox txtDescripcion = new TextBox();
        TextBox txtCategoria = new TextBox();
        NumericUpDown nudPrecio = new NumericUpDown{ DecimalPlaces=2, Maximum=1000000 };
        NumericUpDown nudStock = new NumericUpDown{ Maximum=1000000 };
        NumericUpDown nudStockMin = new NumericUpDown{ Maximum=1000000 };
        Button btnOk = new Button{ Text="Guardar"};
        Button btnCancel = new Button{ Text="Cancelar"};

        public ProductEditForm(Product p, bool esNuevo)
        {
            this.esNuevo = esNuevo;
            Producto = new Product
            {
                IDProducto = p.IDProducto,
                Codigo = p.Codigo,
                Descripcion = p.Descripcion,
                Categoria = p.Categoria,
                Precio = p.Precio,
                Stock = p.Stock,
                StockMin = p.StockMin
            };

            Text = esNuevo ? "Nuevo Producto" : "Editar Producto";
            Width = 400; Height = 400;

            var layout = new TableLayoutPanel{ Dock = DockStyle.Fill, ColumnCount=2, RowCount=7, AutoSize=true};
            layout.Controls.Add(new Label{ Text="Código"}, 0,0); layout.Controls.Add(txtCodigo,1,0);
            layout.Controls.Add(new Label{ Text="Descripción"}, 0,1); layout.Controls.Add(txtDescripcion,1,1);
            layout.Controls.Add(new Label{ Text="Categoría"}, 0,2); layout.Controls.Add(txtCategoria,1,2);
            layout.Controls.Add(new Label{ Text="Precio"}, 0,3); layout.Controls.Add(nudPrecio,1,3);
            layout.Controls.Add(new Label{ Text="Stock"}, 0,4); layout.Controls.Add(nudStock,1,4);
            layout.Controls.Add(new Label{ Text="Stock Mínimo"}, 0,5); layout.Controls.Add(nudStockMin,1,5);

            var buttons = new FlowLayoutPanel{ Dock = DockStyle.Bottom, AutoSize=true};
            buttons.Controls.AddRange(new Control[]{ btnOk, btnCancel });

            Controls.Add(layout); Controls.Add(buttons);

            // Inicializar campos
            txtCodigo.Text = Producto.Codigo;
            txtDescripcion.Text = Producto.Descripcion;
            txtCategoria.Text = Producto.Categoria;
            nudPrecio.Value = Producto.Precio;
            nudStock.Value = Producto.Stock;
            nudStockMin.Value = Producto.StockMin;

            btnOk.Click += (s,e)=> {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtDescripcion.Text))
                {
                    MessageBox.Show("Código y Descripción son obligatorios"); return;
                }
                Producto.Codigo = txtCodigo.Text.Trim();
                Producto.Descripcion = txtDescripcion.Text.Trim();
                Producto.Categoria = txtCategoria.Text.Trim();
                Producto.Precio = nudPrecio.Value;
                Producto.Stock = (int)nudStock.Value;
                Producto.StockMin = (int)nudStockMin.Value;
                DialogResult = DialogResult.OK;
            };
            btnCancel.Click += (s,e)=> DialogResult = DialogResult.Cancel;
        }
    }
}
