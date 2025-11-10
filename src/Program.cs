using System.Globalization;

namespace Minimarket
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Configurar cultura argentina globalmente
            var culture = new CultureInfo("es-AR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            
            ApplicationConfiguration.Initialize();
            var userRepo = new Minimarket.Data.UserRepository();
            using var login = new UI.LoginForm(userRepo);
            if (login.ShowDialog() == DialogResult.OK && login.UsuarioLogueado != null)
            {
                Session.CurrentUser = login.UsuarioLogueado;
                Application.Run(new UI.MainForm(login.UsuarioLogueado));
            }
        }
    }
}
