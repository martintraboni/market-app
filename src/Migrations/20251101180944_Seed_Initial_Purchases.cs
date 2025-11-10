using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Initial_Purchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Compras
            migrationBuilder.InsertData(
                table: "Compras",
                columns: new[] { "Id", "SupplierId", "Date", "DocNumber", "Total" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 11, 1, 10, 0, 0, DateTimeKind.Utc), "F0001-00000001", 15000.00m },
                    { 2, 2, new DateTime(2025, 11, 1, 11, 0, 0, DateTimeKind.Utc), "F0002-00000001", 8000.00m }
                }
            );

            // DetalleCompras
            migrationBuilder.InsertData(
                table: "DetalleCompras",
                columns: new[] { "Id", "PurchaseId", "ProductId", "Qty", "Cost", "Subtotal" },
                values: new object[,]
                {
                    { 1, 1, 1, 20, 500.00m, 10000.00m },
                    { 2, 1, 2, 10, 300.00m, 3000.00m },
                    { 3, 1, 3, 5, 200.00m, 1000.00m },
                    { 4, 2, 4, 8, 400.00m, 3200.00m },
                    { 5, 2, 5, 16, 150.00m, 2400.00m },
                    { 6, 2, 1, 8, 500.00m, 4000.00m }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DetalleCompras",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6 }
            );

            migrationBuilder.DeleteData(
                table: "Compras",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 }
            );
        }
    }
}
