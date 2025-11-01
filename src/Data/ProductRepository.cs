using Models;

namespace Minimarket.Data
{
    public static class ProductRepository
    {
        public static List<Minimarket.DTOs.ProductListDto> GetAllDto(string filtro = "")
        {
            using var db = new MinimarketContext();
            var query = db.Productos
                .Where(p => (string.IsNullOrEmpty(filtro) || p.Code.Contains(filtro) || p.Name.Contains(filtro)))
                .Select(p => new Minimarket.DTOs.ProductListDto
                {
                    Code = p.Code,
                    Name = p.Name,
                    Category = p.Category != null ? p.Category.Name : "",
                    Cost = p.Cost,
                    Price = p.Price,
                    Stock = p.Stock,
                    MinStock = p.MinStock,
                    IsActive = p.IsActive
                });
            return query.ToList();
        }

        public static List<Product> GetAll()
        {
            using var db = new MinimarketContext();
            return db.Productos.ToList();
        }

        public static void ChangeActiveStatus(string codigo)
        {
            using var db = new MinimarketContext();
            var producto = db.Productos.FirstOrDefault(p => p.Code == codigo);
            if (producto != null)
            {
                producto.IsActive = !producto.IsActive;
                db.SaveChanges();
            }
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
