using System;
using System.Windows.Forms;
using Models;

namespace Minimarket.UI
{
    public class CategoryEditForm : Form
    {
    private TextBox txtNombre = new TextBox { Left = 120, Top = 20, Width = 200 };
    private Button btnGuardar = new Button { Text = "Guardar", Left = 120, Top = 60, Width = 80 };
    private Button btnCancelar = new Button { Text = "Cancelar", Left = 240, Top = 60, Width = 80 };
    private Category categoria;

        public CategoryEditForm() : this(null) { }

        public CategoryEditForm(Category categoria)
        {
            this.categoria = categoria;
            Text = categoria == null ? "Agregar Categoría" : "Editar Categoría";
            Width = 380;
            Height = 200;
            StartPosition = FormStartPosition.CenterParent;


            Controls.Add(new Label { Text = "Nombre:", Left = 20, Top = 20, Width = 100 });
            Controls.Add(txtNombre);
            Controls.Add(btnGuardar);
            Controls.Add(btnCancelar);

            if (categoria != null)
            {
                txtNombre.Text = categoria.Name;
            }

            btnGuardar.Click += (s, e) => Guardar();
            btnCancelar.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }

            using var db = new MinimarketContext();
            if (categoria == null)
            {
                var nueva = new Category { Name = txtNombre.Text };
                db.Categorias.Add(nueva);
            }
            else
            {
                var catDb = db.Categorias.Find(categoria.Id);
                if (catDb != null)
                {
                    catDb.Name = txtNombre.Text;
                }
            }
            db.SaveChanges();
            DialogResult = DialogResult.OK;
        }
    }
}
