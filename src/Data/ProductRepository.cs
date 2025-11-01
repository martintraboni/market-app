using Models;

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
                    .Where(p => p.Code.Contains(filtro) || p.Name.Contains(filtro))
                    .ToList();
        }

        public static Product? GetByCodigo(string codigo)
        {
            using var db = new MinimarketContext();
            return db.Productos.FirstOrDefault(p => p.Code == codigo);
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
            var existing = db.Productos.FirstOrDefault(x => x.Code == p.Code);
            if (existing != null)
            {
                existing.Name = p.Name;
                existing.CategoryId = p.CategoryId;
                existing.Price = p.Price;
                existing.Stock = p.Stock;
                existing.MinStock = p.MinStock;
                db.SaveChanges();
            }
        }

        public static void DeleteByCodigo(string codigo)
        {
            using var db = new MinimarketContext();
            var producto = db.Productos.FirstOrDefault(p => p.Code == codigo);
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
