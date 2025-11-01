namespace Minimarket
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var userRepo = new Minimarket.Data.UserRepository();
            using var login = new UI.LoginForm(userRepo);
            if (login.ShowDialog() == DialogResult.OK && login.UsuarioLogueado != null)
            {
                Application.Run(new UI.MainForm(login.UsuarioLogueado));
            }
        }
    }
}
