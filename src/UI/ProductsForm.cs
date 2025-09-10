using System;
using System.Linq;
using System.Windows.Forms;
using Minimarket.Data;
using Minimarket.Models;

namespace Minimarket.UI
{
    public class ProductsForm : Form
    {
        private DataGridView grid = new DataGridView{ Dock = DockStyle.Fill, ReadOnly=true, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
        private TextBox txtFiltro = new TextBox{ PlaceholderText="Buscar por código o descripción"};
        private Button btnBuscar = new Button{ Text="Buscar"};
        private Button btnAgregar = new Button{ Text="Agregar"};
        private Button btnEditar = new Button{ Text="Editar"};
        private Button btnEliminar = new Button{ Text="Eliminar"};

        public ProductsForm()
        {
            Text = "Productos";
            Width = 900; Height = 600;

            var top = new FlowLayoutPanel{ Dock = DockStyle.Top, AutoSize=true};
            top.Controls.AddRange(new Control[]{ txtFiltro, btnBuscar, btnAgregar, btnEditar, btnEliminar });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s,e)=> Cargar();
            btnBuscar.Click += (s,e)=> Cargar(txtFiltro.Text);
            btnAgregar.Click += (s,e)=> Editar(new Product(), true);
            btnEditar.Click += (s,e)=> {
                if (grid.CurrentRow == null) return;
                var p = (Product)grid.CurrentRow.DataBoundItem;
                Editar(p, false);
            };
            btnEliminar.Click += (s,e)=> {
                if (grid.CurrentRow == null) return;
                var p = (Product)grid.CurrentRow.DataBoundItem;
                if (MessageBox.Show($"¿Eliminar {p.Descripcion}?", "Confirmar", MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    ProductRepository.DeleteByCodigo(p.Codigo);
                    Cargar(txtFiltro.Text);
                }
            };
        }

        private void Cargar(string filtro="")
        {
            var list = ProductRepository.GetAll(filtro);
            grid.DataSource = list;
        }

        private void Editar(Product p, bool esNuevo)
        {
            var f = new ProductEditForm(p, esNuevo);
            if (f.ShowDialog()==DialogResult.OK)
            {
                if (esNuevo) ProductRepository.Insert(f.Producto);
                else ProductRepository.Update(f.Producto);
                Cargar(txtFiltro.Text);
            }
        }
    }
}
