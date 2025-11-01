using System.Windows.Forms;
using Minimarket.DTOs;

namespace Minimarket.UI
{
    public class SupplierEditForm : Form
    {
        public string Nombre => txtNombre.Text.Trim();
        public string CUIT => txtCUIT.Text.Trim();
        public string Telefono => txtTelefono.Text.Trim();
        public string Email => txtEmail.Text.Trim();

        private TextBox txtNombre = new TextBox();
        private TextBox txtCUIT = new TextBox();
        private TextBox txtTelefono = new TextBox();
        private TextBox txtEmail = new TextBox();
        private Button btnOk = new Button { Text = "Guardar" };
        private Button btnCancel = new Button { Text = "Cancelar" };

        public SupplierEditForm(SupplierListDto? dto = null)
        {
            Text = dto == null ? "Nuevo Proveedor" : "Editar Proveedor";
            Width = 350; Height = 250;
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, AutoSize = true };
            layout.Controls.Add(new Label { Text = "Nombre" }, 0, 0); layout.Controls.Add(txtNombre, 1, 0);
            layout.Controls.Add(new Label { Text = "CUIT" }, 0, 1); layout.Controls.Add(txtCUIT, 1, 1);
            layout.Controls.Add(new Label { Text = "Teléfono" }, 0, 2); layout.Controls.Add(txtTelefono, 1, 2);
            layout.Controls.Add(new Label { Text = "Email" }, 0, 3); layout.Controls.Add(txtEmail, 1, 3);
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true };
            buttons.Controls.AddRange(new Control[] { btnOk, btnCancel });
            Controls.Add(layout); Controls.Add(buttons);
            if (dto != null)
            {
                txtNombre.Text = dto.Name;
                txtCUIT.Text = dto.CUIT;
                txtTelefono.Text = dto.Phone;
                txtEmail.Text = dto.Email;
            }
            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCUIT.Text))
                {
                    MessageBox.Show("Nombre y CUIT son obligatorios");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txtTelefono.Text) && !System.Text.RegularExpressions.Regex.IsMatch(txtTelefono.Text, @"^\+?\d{7,15}$"))
                {
                    MessageBox.Show("Ingrese un teléfono válido (solo números, puede incluir +, 7 a 15 dígitos)");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Ingrese un email válido");
                    return;
                }
                DialogResult = DialogResult.OK;
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }
    }
}
