using System;
using System.Linq;
using System.Windows.Forms;
using Minimarket.Data;

namespace Minimarket.UI
{
    public class UsersForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        private Button btnNuevo = new Button { Text = "Nuevo Usuario", AutoSize = true };
        private Button btnEditar = new Button { Text = "Editar", AutoSize = true };
        private Button btnToggleActivo = new Button { Text = "Habilitar/Inhabilitar", AutoSize = true };
        private Button btnEliminar = new Button { Text = "Eliminar", AutoSize = true };

        public UsersForm()
        {
            Text = "Gestión de Usuarios";
            Width = 900; Height = 600;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }

            // Panel superior con botones
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight
            };

            btnNuevo.Margin = new Padding(0, 0, 10, 0);
            btnEditar.Margin = new Padding(0, 0, 10, 0);
            btnToggleActivo.Margin = new Padding(0, 0, 10, 0);
            btnEliminar.Margin = new Padding(0, 0, 10, 0);

            topPanel.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnToggleActivo, btnEliminar });

            // Configurar grid con columnas en español
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", DataPropertyName = "Username", HeaderText = "Usuario", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", DataPropertyName = "FullName", HeaderText = "Nombre Completo", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", DataPropertyName = "Role", HeaderText = "Rol", Width = 120 });
            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "IsActive", DataPropertyName = "IsActive", HeaderText = "Activo", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreatedAt", DataPropertyName = "CreatedAt", HeaderText = "Fecha Creación", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });

            Controls.Add(grid);
            Controls.Add(topPanel);

            btnNuevo.Click += (s, e) => NuevoUsuario();
            btnEditar.Click += (s, e) => EditarUsuario();
            btnToggleActivo.Click += (s, e) => ToggleActivo();
            btnEliminar.Click += (s, e) => EliminarUsuario();
            Load += (s, e) => Cargar();

            // Agregar tooltips
            var toolTip = new ToolTip();
            toolTip.SetToolTip(btnNuevo, "Crear un nuevo usuario en el sistema.");
            toolTip.SetToolTip(btnEditar, "Editar el usuario seleccionado.");
            toolTip.SetToolTip(btnToggleActivo, "Habilitar o inhabilitar el usuario seleccionado.");
            toolTip.SetToolTip(btnEliminar, "Eliminar permanentemente el usuario seleccionado.");
        }

        private void Cargar()
        {
            grid.DataSource = UserRepository.GetAllDto();
        }

        private void NuevoUsuario()
        {
            var form = new UserEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void EditarUsuario()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int userId = (int)grid.CurrentRow.Cells["Id"].Value;
            var form = new UserEditForm(userId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void ToggleActivo()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int userId = (int)grid.CurrentRow.Cells["Id"].Value;
            string username = grid.CurrentRow.Cells["Username"].Value.ToString();
            bool isActive = (bool)grid.CurrentRow.Cells["IsActive"].Value;
            string accion = isActive ? "inhabilitar" : "habilitar";

            if (MessageBox.Show($"¿Está seguro que desea {accion} al usuario '{username}'?", "Confirmar", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    UserRepository.ToggleActive(userId, Session.CurrentUser.Id);
                    MessageBox.Show("Estado actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Cargar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cambiar estado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EliminarUsuario()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int userId = (int)grid.CurrentRow.Cells["Id"].Value;
            string username = grid.CurrentRow.Cells["Username"].Value.ToString();

            if (userId == Session.CurrentUser.Id)
            {
                MessageBox.Show("No puede eliminar su propio usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"¿Está seguro que desea eliminar al usuario '{username}'? Esta acción no se puede deshacer.", "Confirmar", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    UserRepository.DeleteUser(userId, Session.CurrentUser.Id);
                    MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Cargar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
