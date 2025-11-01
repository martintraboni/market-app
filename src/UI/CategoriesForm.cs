using Minimarket.DTOs;
using MiniMarket.Data;
using Models;

namespace Minimarket.UI
{
    public class CategoriesForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private TextBox txtNombre = new TextBox { PlaceholderText = "Nombre de categoría" };
        private Button btnAgregar = new Button { Text = "Agregar" };
        private Button btnEditar = new Button { Text = "Editar" };
        private Button btnEliminar = new Button { Text = "Eliminar" };

        public CategoriesForm()
        {
            Text = "Categorías";
            Width = 500; Height = 400;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            top.Controls.AddRange(new Control[] { txtNombre, btnAgregar, btnEditar, btnEliminar });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Agregar();
            btnEditar.Click += (s, e) => Editar();
            btnEliminar.Click += (s, e) => Eliminar();
        }

        private void Cargar()
        {
            grid.DataSource = CategoryRepository.GetAllDto();
        }

        private void Agregar()
        {
            var nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre es obligatorio");
                return;
            }
            using var db = new MinimarketContext();
            db.Categorias.Add(new Category { Name = nombre });
            db.SaveChanges();
            Cargar();
            txtNombre.Clear();
        }

        private void Editar()
        {
            if (grid.CurrentRow == null) return;
            var cat = (CategoryListDto)grid.CurrentRow.DataBoundItem;
            var nuevoNombre = Microsoft.VisualBasic.Interaction.InputBox("Nuevo nombre:", "Editar Categoría", cat.Name);
            if (!string.IsNullOrWhiteSpace(nuevoNombre))
            {
                using var db = new MinimarketContext();
                var c = db.Categorias.FirstOrDefault(x => x.Id == cat.Id);
                if (c != null)
                {
                    c.Name = nuevoNombre;
                    db.SaveChanges();
                    Cargar();
                }
            }
        }

        private void Eliminar()
        {
            if (grid.CurrentRow == null) return;
            var cat = (CategoryListDto)grid.CurrentRow.DataBoundItem;
            if (MessageBox.Show($"¿Eliminar {cat.Name}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using var db = new MinimarketContext();
                var c = db.Categorias.FirstOrDefault(x => x.Id == cat.Id);
                if (c != null)
                {
                    db.Categorias.Remove(c);
                    db.SaveChanges();
                    Cargar();
                }
            }
        }
    }
}