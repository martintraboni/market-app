using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Minimarket.Models;

namespace Minimarket.Data
{
    public static class ProductRepository
    {
        public static List<Product> GetAll(string filtro = "")
        {
            string sql = "SELECT * FROM Productos";
            if (!string.IsNullOrWhiteSpace(filtro))
                sql += " WHERE Codigo LIKE @f OR Descripcion LIKE @f";
            var param = string.IsNullOrWhiteSpace(filtro) ? null :
                new[] { new SqlParameter("@f", $"%{filtro}%") };
            var dt = Db.Query(sql, param ?? new SqlParameter[] { });
            var list = new List<Product>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Product
                {
                    IDProducto = (int)row["IDProducto"],
                    Codigo = row["Codigo"].ToString()!,
                    Descripcion = row["Descripcion"].ToString()!,
                    Categoria = row["Categoria"].ToString()!,
                    Precio = (decimal)row["Precio"],
                    Stock = (int)row["Stock"],
                    StockMin = (int)row["StockMin"]
                });
            }
            return list;
        }

        public static Product? GetByCodigo(string codigo)
        {
            string sql = "SELECT TOP 1 * FROM Productos WHERE Codigo=@c";
            var dt = Db.Query(sql, new SqlParameter("@c", codigo));
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new Product
            {
                IDProducto = (int)row["IDProducto"],
                Codigo = row["Codigo"].ToString()!,
                Descripcion = row["Descripcion"].ToString()!,
                Categoria = row["Categoria"].ToString()!,
                Precio = (decimal)row["Precio"],
                Stock = (int)row["Stock"],
                StockMin = (int)row["StockMin"]
            };
        }

        public static void Insert(Product p)
        {
            string sql = @"INSERT INTO Productos (Codigo, Descripcion, Categoria, Precio, Stock, StockMin)
                           VALUES (@Codigo, @Descripcion, @Categoria, @Precio, @Stock, @StockMin)";
            Db.Execute(sql,
                new SqlParameter("@Codigo", p.Codigo),
                new SqlParameter("@Descripcion", p.Descripcion),
                new SqlParameter("@Categoria", p.Categoria),
                new SqlParameter("@Precio", p.Precio),
                new SqlParameter("@Stock", p.Stock),
                new SqlParameter("@StockMin", p.StockMin));
        }

        public static void Update(Product p)
        {
            string sql = @"UPDATE Productos SET Descripcion=@Descripcion, Categoria=@Categoria, Precio=@Precio,
                           Stock=@Stock, StockMin=@StockMin WHERE Codigo=@Codigo";
            Db.Execute(sql,
                new SqlParameter("@Descripcion", p.Descripcion),
                new SqlParameter("@Categoria", p.Categoria),
                new SqlParameter("@Precio", p.Precio),
                new SqlParameter("@Stock", p.Stock),
                new SqlParameter("@StockMin", p.StockMin),
                new SqlParameter("@Codigo", p.Codigo));
        }

        public static void DeleteByCodigo(string codigo)
        {
            string sql = "DELETE FROM Productos WHERE Codigo=@Codigo";
            Db.Execute(sql, new SqlParameter("@Codigo", codigo));
        }

        public static void DescontarStock(int idProducto, int cantidad)
        {
            string sql = "UPDATE Productos SET Stock = Stock - @cant WHERE IDProducto=@id";
            Db.Execute(sql,
                new SqlParameter("@cant", cantidad),
                new SqlParameter("@id", idProducto));
        }
    }
}
