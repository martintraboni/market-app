namespace Minimarket
{
    public static class Config
    {
        // Ajustar la cadena de conexión a su entorno:
        // Opción 1: LocalDB (Visual Studio)
        // public static string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=MinimarketDB;Trusted_Connection=True;";
        // Opción 2: SQL Server Express
        // public static string ConnectionString = @"Server=.\SQLEXPRESS;Database=MinimarketDB;Trusted_Connection=True;TrustServerCertificate=True;";
        // Opción 3: Servidor local
        public static string ConnectionString = @"Server=localhost;Database=MinimarketDB;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
