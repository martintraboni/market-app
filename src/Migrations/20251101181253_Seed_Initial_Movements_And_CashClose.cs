using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Initial_Movements_And_CashClose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Movimientos de Inventario
            migrationBuilder.InsertData(
                table: "MovimientosInventario",
                columns: new[] { "Id", "ProductId", "DateTime", "Type", "Qty", "Reason", "RefId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 11, 1, 10, 5, 0, DateTimeKind.Utc), "Ingreso", 20, "Compra", 1 },
                    { 2, 2, new DateTime(2025, 11, 1, 10, 5, 0, DateTimeKind.Utc), "Ingreso", 10, "Compra", 1 },
                    { 3, 1, new DateTime(2025, 11, 1, 12, 5, 0, DateTimeKind.Utc), "Egreso", 2, "Venta", 1 },
                    { 4, 2, new DateTime(2025, 11, 1, 12, 5, 0, DateTimeKind.Utc), "Egreso", 1, "Venta", 1 }
                }
            );

            // Movimientos de Caja
            migrationBuilder.InsertData(
                table: "MovimientosCaja",
                columns: new[] { "Id", "DateTime", "Type", "Amount", "Concept", "UserId", "SaleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 1, 12, 10, 0, DateTimeKind.Utc), "Ingreso", 2000.00m, "Venta 1", 1, 1 },
                    { 2, new DateTime(2025, 11, 1, 13, 10, 0, DateTimeKind.Utc), "Ingreso", 1500.00m, "Venta 2", 1, 2 }
                }
            );

            // Cierre de Caja
            migrationBuilder.InsertData(
                table: "CierresCaja",
                columns: new[] { "Id", "Date", "UserId", "CashInHand", "PosTotal", "SystemTotal", "Difference", "Notes" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 1, 23, 59, 0, DateTimeKind.Utc), 1, 3500.00m, 0.00m, 3500.00m, 0.00m, "Cierre automático de ejemplo" }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MovimientosInventario",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4 }
            );

            migrationBuilder.DeleteData(
                table: "MovimientosCaja",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 }
            );

            migrationBuilder.DeleteData(
                table: "CierresCaja",
                keyColumn: "Id",
                keyValue: 1
            );
        }
    }
}
