using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    public partial class Seed_Initial_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Username", "Password", "Role", "FullName", "IsActive", "CreatedAt" },
                values: new object[] { 1, "admin", "admin123", "Admin", "Administrador", true, DateTime.UtcNow }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1
            );
        }
    }
}
