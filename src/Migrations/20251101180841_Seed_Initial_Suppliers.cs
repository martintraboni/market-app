using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Initial_Suppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "Id", "Name", "CUIT", "Phone", "Email" },
                values: new object[,]
                {
                    { 1, "Distribuidora Centro", "30-12345678-9", "+5491122334455", "centro@proveedores.com" },
                    { 2, "Mayorista Norte", "30-87654321-0", "+5491166778899", "norte@proveedores.com" },
                    { 3, "Alimentos S.A.", "30-11223344-5", "+5491133557799", "alimentos@proveedores.com" }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Proveedores",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 }
            );
        }
    }
}
