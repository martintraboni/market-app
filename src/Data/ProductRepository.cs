using Minimarket.Models;

namespace Minimarket.Data
{
    public static class ProductRepository
    {
        public static List<Product> GetAll(string filtro = "")
        {
            using var db = new MinimarketContext();
            return string.IsNullOrEmpty(filtro)
                ? db.Productos.ToList()
                : db.Productos
                    .Where(p => p.Codigo.Contains(filtro) || p.Descripcion.Contains(filtro))
                    .ToList();
        }

        public static Product? GetByCodigo(string codigo)
        {
            using var db = new MinimarketContext();
            return db.Productos.FirstOrDefault(p => p.Codigo == codigo);
        }

        public static void Insert(Product p)
        {
            using var db = new MinimarketContext();
            db.Productos.Add(p);
            db.SaveChanges();
        }

        public static void Update(Product p)
        {
            using var db = new MinimarketContext();
            var existing = db.Productos.FirstOrDefault(x => x.Codigo == p.Codigo);
            if (existing != null)
            {
                existing.Descripcion = p.Descripcion;
                existing.Categoria = p.Categoria;
                existing.Precio = p.Precio;
                existing.Stock = p.Stock;
                existing.StockMin = p.StockMin;
                db.SaveChanges();
            }
        }

        public static void DeleteByCodigo(string codigo)
        {
            using var db = new MinimarketContext();
            var producto = db.Productos.FirstOrDefault(p => p.Codigo == codigo);
            if (producto != null)
            {
                db.Productos.Remove(producto);
                db.SaveChanges();
            }
        }

        public static void DescontarStock(int idProducto, int cantidad)
        {
            using var db = new MinimarketContext();
            var producto = db.Productos.FirstOrDefault(p => p.Id == idProducto);
            if (producto != null)
            {
                producto.Stock -= cantidad;
                db.SaveChanges();
            }
        }
    }
}
