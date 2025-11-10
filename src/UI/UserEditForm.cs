using System;
using System.Linq;
using System.Windows.Forms;
using Minimarket.Data;
using Models;

namespace Minimarket.UI
{
    public class UserEditForm : Form
    {
        private int? _userId;
        private TextBox txtUsername = new TextBox { Width = 250 };
        private TextBox txtFullName = new TextBox { Width = 250 };
        private TextBox txtPassword = new TextBox { Width = 250, UseSystemPasswordChar = true };
        private ComboBox cboRole = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
        private CheckBox chkIsActive = new CheckBox { Text = "Usuario Activo", Checked = true };
        private Button btnGuardar = new Button { Text = "Guardar", Width = 100 };
        private Button btnCancelar = new Button { Text = "Cancelar", Width = 100 };

        public UserEditForm(int? userId)
        {
            _userId = userId;
            Text = userId.HasValue ? "Editar Usuario" : "Nuevo Usuario";
            Width = 400; Height = 350;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                WrapContents = false
            };

            panel.Controls.Add(new Label { Text = "Usuario:", AutoSize = true });
            panel.Controls.Add(txtUsername);
            panel.Controls.Add(new Label { Text = "Nombre Completo:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            panel.Controls.Add(txtFullName);
            panel.Controls.Add(new Label { Text = userId.HasValue ? "Contraseña (dejar vacío para no cambiar):" : "Contraseña:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            panel.Controls.Add(txtPassword);
            panel.Controls.Add(new Label { Text = "Rol:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            panel.Controls.Add(cboRole);
            chkIsActive.Margin = new Padding(0, 10, 0, 0);
            panel.Controls.Add(chkIsActive);

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 15, 0, 0)
            };
            btnGuardar.Margin = new Padding(0, 0, 10, 0);
            buttonPanel.Controls.Add(btnGuardar);
            buttonPanel.Controls.Add(btnCancelar);
            panel.Controls.Add(buttonPanel);

            Controls.Add(panel);

            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Load += UserEditForm_Load;

            // Tooltips
            var toolTip = new ToolTip();
            toolTip.SetToolTip(txtUsername, "Nombre de usuario para iniciar sesión. Debe ser único.");
            toolTip.SetToolTip(txtFullName, "Nombre completo del usuario.");
            toolTip.SetToolTip(txtPassword, userId.HasValue ? "Dejar en blanco para mantener la contraseña actual." : "Contraseña para iniciar sesión.");
            toolTip.SetToolTip(cboRole, "Rol del usuario: Admin tiene acceso completo, Usuario tiene acceso estándar.");
            toolTip.SetToolTip(chkIsActive, "Si está desmarcado, el usuario no podrá iniciar sesión.");
        }

        private void UserEditForm_Load(object sender, EventArgs e)
        {
            // Cargar roles
            var roles = RoleRepository.GetAll();
            cboRole.DisplayMember = "RoleDescription";
            cboRole.ValueMember = "Id";
            cboRole.DataSource = roles;

            if (_userId.HasValue)
            {
                // Modo edición - cargar datos del usuario
                using var db = new MinimarketContext();
                var user = db.Usuarios.FirstOrDefault(u => u.Id == _userId.Value);
                if (user != null)
                {
                    txtUsername.Text = user.Username;
                    txtFullName.Text = user.FullName;
                    txtPassword.Text = "";
                    cboRole.SelectedValue = user.RoleId;
                    chkIsActive.Checked = user.IsActive;
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Debe ingresar un nombre de usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Debe ingresar el nombre completo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            if (!_userId.HasValue && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Debe ingresar una contraseña para el nuevo usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (cboRole.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un rol.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int roleId = (int)cboRole.SelectedValue;

                if (_userId.HasValue)
                {
                    // Actualizar usuario existente
                    UserRepository.UpdateUser(_userId.Value, txtUsername.Text.Trim(), txtFullName.Text.Trim(), 
                        txtPassword.Text.Trim(), roleId, chkIsActive.Checked, Session.CurrentUser.Id);
                    MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Crear nuevo usuario
                    UserRepository.CreateUser(txtUsername.Text.Trim(), txtFullName.Text.Trim(), 
                        txtPassword.Text.Trim(), roleId, Session.CurrentUser.Id);
                    MessageBox.Show("Usuario creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
