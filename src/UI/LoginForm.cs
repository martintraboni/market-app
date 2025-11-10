using Minimarket.Data;
using Minimarket.Exceptions;
using Models;

namespace Minimarket.UI
{
    public class LoginForm : Form
    {
        private TextBox txtUser = new TextBox { PlaceholderText = "Usuario" };
        private TextBox txtPass = new TextBox { PlaceholderText = "Contraseña", UseSystemPasswordChar = true };
        private Button btnLogin = new Button { Text = "Ingresar" };
        public User? UsuarioLogueado { get; private set; }
        private readonly UserRepository _userRepository;
        public LoginForm(UserRepository userRepository)
        {
            _userRepository = userRepository;

            Text = "Login";
            Width = 300;
            Height = 180;
            try { this.Icon = new System.Drawing.Icon("taml.ico"); } catch { }
            var lblUser = new Label { Text = "Usuario", Top = 20, Left = 20, Width = 80 };
            txtUser.Top = 20; txtUser.Left = 110; txtUser.Width = 140;
            var lblPass = new Label { Text = "Contraseña", Top = 60, Left = 20, Width = 80 };
            txtPass.Top = 60; txtPass.Left = 110; txtPass.Width = 140;
            btnLogin.Top = 100; btnLogin.Left = 110; btnLogin.Width = 140;
            btnLogin.Click += BtnLogin_Click;
            Controls.AddRange(new Control[] { lblUser, txtUser, lblPass, txtPass, btnLogin });
        }

        public LoginForm()
        {
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            try
            {
                var user = _userRepository.Login(txtUser.Text, txtPass.Text);
                UsuarioLogueado = user;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (UsuarioNoExisteException)
            {
                MessageBox.Show("El usuario no existe.", "Login fallido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (ContrasenaIncorrectaException)
            {
                MessageBox.Show("La contraseña es incorrecta.", "Login fallido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPass.Clear();
                txtPass.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión o consulta:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
