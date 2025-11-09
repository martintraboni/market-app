using Minimarket.Data;
using Models;

namespace Minimarket.UI
{
    public class ProductsForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private TextBox txtFiltro = new TextBox { PlaceholderText = "Buscar por código o descripción" };
        private Button btnBuscar = new Button { Text = "Buscar" };
        private Button btnAgregar = new Button { Text = "Agregar" };
        private Button btnEditar = new Button { Text = "Editar" };
        private Button btnInhabilitar = new Button { Text = "Activar/Desactivar" };

        public ProductsForm()
        {
            Text = "Productos";
            Width = 900; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }


            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            top.Controls.AddRange(new Control[] { txtFiltro, btnBuscar, btnAgregar, btnEditar, btnInhabilitar });

            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Code", HeaderText = "Código" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nombre" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Category", HeaderText = "Categoría" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cost", HeaderText = "Costo" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Precio" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Stock", HeaderText = "Stock" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MinStock", HeaderText = "Stock Mínimo" });
            grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsActive", HeaderText = "Activo" });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnBuscar.Click += (s, e) => Cargar(txtFiltro.Text);
            btnAgregar.Click += (s, e) => Editar(new Product(), true);
            btnEditar.Click += (s, e) =>
            {
                if (grid.CurrentRow == null) return;
                var dto = grid.CurrentRow.DataBoundItem as Minimarket.DTOs.ProductListDto;
                if (dto == null) return;
                var p = ProductRepository.GetByCodigo(dto.Code);
                if (p == null) return;
                Editar(p, false);
            };
            btnInhabilitar.AutoSize = true;
            btnInhabilitar.Click += (s, e) =>
            {
                if (grid.CurrentRow == null) return;
                var dto = grid.CurrentRow.DataBoundItem as Minimarket.DTOs.ProductListDto;
                if (dto == null) return;
                if (MessageBox.Show($"¿Desea cambiar el estado de {dto.Name}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ProductRepository.ChangeActiveStatus(dto.Code);
                    Cargar(txtFiltro.Text);
                }
            };
        }

        private void Cargar(string filtro = "")
        {
            var list = ProductRepository.GetAllDto(filtro);
            grid.DataSource = list;
        }

        private void Editar(Product p, bool esNuevo)
        {
            var f = new ProductEditForm(p, esNuevo);
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (esNuevo) ProductRepository.Insert(f.Producto);
                else ProductRepository.Update(f.Producto);
                Cargar(txtFiltro.Text);
            }
        }
    }
}
