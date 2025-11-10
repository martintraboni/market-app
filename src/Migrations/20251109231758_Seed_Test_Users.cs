using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Test_Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Usuario de prueba con rol User (Cajero)
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Username", "Password", "FullName", "RoleId", "IsActive", "CreatedAt" },
                values: new object[] { "cajero", "cajero123", "Juan Pérez (Cajero)", 2, true, DateTime.Now });

            // Usuario de prueba con rol Supervisor
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Username", "Password", "FullName", "RoleId", "IsActive", "CreatedAt" },
                values: new object[] { "supervisor", "super123", "María García (Supervisor)", 3, true, DateTime.Now });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar usuarios de prueba
            migrationBuilder.Sql("DELETE FROM Usuarios WHERE Username IN ('cajero', 'supervisor')");
        }
    }
}
