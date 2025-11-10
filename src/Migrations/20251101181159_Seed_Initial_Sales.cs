using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Initial_Sales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ventas
            migrationBuilder.InsertData(
                table: "Ventas",
                columns: new[] { "Id", "DateTime", "UserId", "PaymentMethod", "Total" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 1, 12, 0, 0, DateTimeKind.Utc), 1, "Efectivo", 2000.00m },
                    { 2, new DateTime(2025, 11, 1, 13, 0, 0, DateTimeKind.Utc), 1, "Tarjeta", 1500.00m }
                }
            );

            // Items (detalle de ventas)
            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "SaleId", "ProductId", "Qty", "UnitPrice", "Subtotal", "Code", "Descripcion" },
                values: new object[,]
                {
                    { 1, 1, 1, 2, 750.00m, 1500.00m, "AL001", "Arroz 1kg" },
                    { 2, 1, 2, 1, 450.00m, 450.00m, "AL002", "Fideos 500g" },
                    { 3, 2, 3, 3, 350.00m, 1050.00m, "BE001", "Agua Mineral 1.5L" },
                    { 4, 2, 5, 3, 150.00m, 450.00m, "LI001", "Lavandina 1L" }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4 }
            );

            migrationBuilder.DeleteData(
                table: "Ventas",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 }
            );
        }
    }
}
