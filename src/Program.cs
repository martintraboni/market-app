namespace Minimarket
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var login = new UI.LoginForm();
            if (login.ShowDialog() == DialogResult.OK && login.UsuarioLogueado != null)
            {
                Application.Run(new UI.MainForm(login.UsuarioLogueado));
            }
        }
    }
}
