using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Initial_Prods_And_Cats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                 table: "Categorias",
                 columns: new[] { "Id", "Name" },
                 values: new object[,]
                 {
                        { 1, "Alimentos" },
                        { 2, "Bebidas" },
                        { 3, "Limpieza" }
                 }
            );

            migrationBuilder.InsertData(
              table: "Productos",
              columns: new[] { "Id", "Code", "Name", "CategoryId", "Cost", "Price", "Stock", "MinStock", "IsActive" },
              values: new object[,]
              {
                    { 1, "AL001", "Arroz 1kg", 1, 500.00m, 750.00m, 100, 10, true },
                    { 2, "AL002", "Fideos 500g", 1, 300.00m, 450.00m, 80, 10, true },
                    { 3, "BE001", "Agua Mineral 1.5L", 2, 200.00m, 350.00m, 120, 15, true },
                    { 4, "BE002", "Gaseosa 2.25L", 2, 400.00m, 650.00m, 60, 8, true },
                    { 5, "LI001", "Lavandina 1L", 3, 150.00m, 300.00m, 50, 5, true }
              }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
            table: "Productos",
            keyColumn: "Id",
            keyValues: new object[] { 1, 2, 3, 4, 5 }
            );


            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 }
            );
        }
    }
}
