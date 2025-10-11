using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimarket.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Codigo", "Descripcion", "Categoria", "Precio", "Stock", "StockMin" },
                values: new object[,]
                {
                    { "001", "Manzanas", "Frutas", 1.20m, 100, 10 },
                    { "002", "Bananas", "Frutas", 0.80m, 150, 15 },
                    { "003", "Leche", "Lácteos", 0.90m, 200, 20 },
                    { "004", "Pan Integral", "Panadería", 1.50m, 80, 8 },
                    { "005", "Arroz", "Cereales", 2.00m, 120, 12 },
                    { "006", "Huevos (docena)", "Lácteos", 2.50m, 60, 6 },
                    { "007", "Pollo (kg)", "Carnes", 5.00m, 50, 5 },
                    { "008", "Carne de Res (kg)", "Carnes", 8.00m, 40, 4 },
                    { "009", "Pasta (500g)", "Cereales", 1.30m, 90, 9 },
                    { "010", "Tomates", "Verduras", 1.10m, 110, 11 },
                    { "011", "Lechuga", "Verduras", 0.70m, 130, 13 },
                    { "012", "Queso", "Lácteos", 3.00m, 70, 7 },
                    { "013", "Yogur", "Lácteos", 1.00m, 140, 14 },
                    { "014", "Cereal", "Cereales", 2.20m, 95, 9 },
                    { "015", "Jugo de Naranja", "Bebidas", 1.80m, 85, 8 },
                    { "016", "Agua Mineral", "Bebidas", 0.50m, 160, 16 },
                    { "017", "Café", "Bebidas", 4.00m, 75, 7 },
                    { "018", "Té", "Bebidas", 3.50m, 65, 6 },
                    { "019", "Galletas", "Snacks", 1.60m, 115, 11 },
                    { "020", "Chocolates", "Snacks", 2.80m, 105, 10 },
                    { "021", "Papas Fritas", "Snacks", 1.40m, 125, 12 },
                    { "022", "Cerveza", "Bebidas", 2.50m, 90, 9 },
                    { "023", "Vino Tinto", "Bebidas", 10.00m, 30, 3 },
                    { "024", "Vino Blanco", "Bebidas", 9.00m, 35, 3 },
                    { "025", "Aceite de Oliva", "Condimentos", 6.00m, 45, 4 },
                    { "026", "Vinagre", "Condimentos", 2.00m, 55, 5 },
                    { "027", "Sal", "Condimentos", 0.60m, 150, 15 },
                    { "028", "Azúcar", "Condimentos", 0.70m, 140, 14 },
                    { "029", "Harina", "Cereales", 1.20m, 130, 13 },
                    { "030", "Mantequilla", "Lácteos", 2.30m, 80, 8 },
                    { "031", "Mandarinas", "Frutas", 1.10m ,40m, 9 },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string[] codigos = new[]
            {
                "001", "002", "003", "004", "005", "006", "007", "008", "009", "010",
                "011", "012", "013", "014", "015", "016", "017", "018", "019", "020",
                "021", "022", "023", "024", "025", "026", "027", "028", "029", "030", "031"
            };

            foreach (var codigo in codigos)
            {
                migrationBuilder.DeleteData(
                    table: "Productos",
                    keyColumn: "Codigo",
                    keyValue: codigo
                );
            }
        }
    }
}
