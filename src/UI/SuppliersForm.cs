using Minimarket.Data;
using Minimarket.DTOs;
using Models;

namespace Minimarket.UI
{
    public class SuppliersForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private TextBox txtNombre = new TextBox { PlaceholderText = "Nombre" };
        private TextBox txtCUIT = new TextBox { PlaceholderText = "CUIT" };
        private TextBox txtTelefono = new TextBox { PlaceholderText = "Teléfono" };
        private TextBox txtEmail = new TextBox { PlaceholderText = "Email" };
        private Button btnAgregar = new Button { Text = "Agregar" };
        private Button btnEditar = new Button { Text = "Editar" };
        private Button btnEliminar = new Button { Text = "Eliminar" };

        public SuppliersForm()
        {
            Text = "Proveedores";
            Width = 700; Height = 400;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            top.Controls.AddRange(new Control[] { txtNombre, txtCUIT, txtTelefono, txtEmail, btnAgregar, btnEditar, btnEliminar });


            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nombre" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CUIT", HeaderText = "CUIT" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", HeaderText = "Teléfono" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email" });

            Controls.Add(grid);
            Controls.Add(top);

            Load += (s, e) => Cargar();
            btnAgregar.Click += (s, e) => Agregar();
            btnEditar.Click += (s, e) => Editar();
            btnEliminar.Click += (s, e) => Eliminar();
        }

        private void Cargar()
        {
            var list = SupplierRepository.GetAll();
            grid.DataSource = list;
        }

        private void Agregar()
        {
            var nombre = txtNombre.Text.Trim();
            var cuit = txtCUIT.Text.Trim();
            var tel = txtTelefono.Text.Trim();
            var email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(cuit))
            {
                MessageBox.Show("Nombre y CUIT son obligatorios");
                return;
            }
            if (!string.IsNullOrWhiteSpace(tel) && !System.Text.RegularExpressions.Regex.IsMatch(tel, @"^\+?\d{7,15}$"))
            {
                MessageBox.Show("Ingrese un teléfono válido (solo números, puede incluir +, 7 a 15 dígitos)");
                return;
            }
            if (!string.IsNullOrWhiteSpace(email) && !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Ingrese un email válido");
                return;
            }
            using var db = new MinimarketContext();
            db.Proveedores.Add(new Supplier { Name = nombre, CUIT = cuit, Phone = tel, Email = email });
            db.SaveChanges();
            Cargar();
            txtNombre.Clear(); txtCUIT.Clear(); txtTelefono.Clear(); txtEmail.Clear();
        }

        private void Editar()
        {
            if (grid.CurrentRow == null) return;
            var prov = (SupplierListDto)grid.CurrentRow.DataBoundItem;
            var editForm = new SupplierEditForm(prov);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                using var db = new MinimarketContext();
                var p = db.Proveedores.FirstOrDefault(x => x.Id == prov.Id);
                if (p != null)
                {
                    p.Name = editForm.Nombre;
                    p.CUIT = editForm.CUIT;
                    p.Phone = editForm.Telefono;
                    p.Email = editForm.Email;
                    db.SaveChanges();
                    Cargar();
                }
            }
        }

        private void Eliminar()
        {
            if (grid.CurrentRow == null) return;
            var prov = (SupplierListDto)grid.CurrentRow.DataBoundItem;
            if (MessageBox.Show($"¿Eliminar {prov.Name}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using var db = new MinimarketContext();
                var p = db.Proveedores.FirstOrDefault(x => x.Id == prov.Id);
                if (p != null)
                {
                    db.Proveedores.Remove(p);
                    db.SaveChanges();
                    Cargar();
                }
            }
        }
    }
}