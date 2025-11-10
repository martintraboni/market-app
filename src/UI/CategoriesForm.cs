using MiniMarket.Data;

namespace Minimarket.UI
{
    public class CategoriesForm : Form
    {
        private DataGridView dgvCategories = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false };
        private Button btnAgregar = new Button { Text = "Agregar" };
        private Button btnEditar = new Button { Text = "Editar" };
        private Button btnEliminar = new Button { Text = "Eliminar" };
        private BindingSource bindingSource = new BindingSource();

        public CategoriesForm()
        {

            Text = "Categorías";
            Width = 600;
            Height = 400;
            StartPosition = FormStartPosition.CenterParent;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            var panelBotones = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            panelBotones.Controls.AddRange(new Control[] { btnAgregar, btnEditar, btnEliminar });

            dgvCategories.AllowUserToAddRows = false;
            Controls.Add(dgvCategories);
            Controls.Add(panelBotones);

            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", FillWeight = 20 });
            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nombre", FillWeight = 80 });

            Load += (s, e) => CargarCategorias();
            btnAgregar.Click += (s, e) => AgregarCategoria();
            btnEditar.Click += (s, e) => EditarCategoria();
            btnEliminar.Click += (s, e) => EliminarCategoria();
        }

        private void CargarCategorias()
        {
            var categorias = CategoryRepository.GetAll();
            bindingSource.DataSource = categorias;
            dgvCategories.DataSource = bindingSource;
        }

        private void AgregarCategoria()
        {
            var form = new CategoryEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CargarCategorias();
            }
        }

        private void EditarCategoria()
        {
            if (bindingSource.Current is Models.Category categoria)
            {
                var form = new CategoryEditForm(categoria);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CargarCategorias();
                }
            }
        }

        private void EliminarCategoria()
        {
            if (bindingSource.Current is Models.Category categoria)
            {
                var result = MessageBox.Show($"¿Está seguro de eliminar la categoría '{categoria.Name}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    CategoryRepository.Delete(categoria.Id);
                    CargarCategorias();
                }
            }
        }
    }
}
