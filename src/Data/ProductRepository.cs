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

        public static List<Product> GetAllForSale()
        {
            using var db = new MinimarketContext();
            return db.Productos
                .Where(p => p.IsActive && p.Stock > 0)
                .OrderBy(p => p.Name)
                .ToList();
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

            // Registrar en auditoría
            db.Auditoria.Add(new AuditLog
            {
                UserId = Session.CurrentUser?.Id ?? 1,
                DateTime = DateTime.Now,
                Event = Constants.AuditEventCreateProduct,
                Details = $"Código: {p.Code} - Nombre: {p.Name} - Precio: {p.Price:C2}"
            });
            db.SaveChanges();
        }

        public static void Update(Product p)
        {
            using var db = new MinimarketContext();
            var existing = db.Productos.FirstOrDefault(x => x.Code == p.Code);
            if (existing != null)
            {
                bool precioCambio = existing.Price != p.Price;
                decimal precioAnterior = existing.Price;
                
                existing.Name = p.Name;
                existing.CategoryId = p.CategoryId;
                existing.Price = p.Price;
                existing.Stock = p.Stock;
                existing.MinStock = p.MinStock;
                db.SaveChanges();

                // Registrar en auditoría
                db.Auditoria.Add(new AuditLog
                {
                    UserId = Session.CurrentUser?.Id ?? 1,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventUpdateProduct,
                    Details = $"Código: {p.Code} - Nombre: {p.Name} - Precio: {p.Price:C2}"
                });
                db.SaveChanges();
                
                // Registrar cambio de precio específico
                if (precioCambio)
                {
                    db.Auditoria.Add(new AuditLog
                    {
                        UserId = Session.CurrentUser?.Id ?? 1,
                        DateTime = DateTime.Now,
                        Event = Constants.AuditEventPriceChange,
                        Details = $"Código: {p.Code} - Nombre: {p.Name} - Precio anterior: {precioAnterior:C2} → Nuevo precio: {p.Price:C2}"
                    });
                    db.SaveChanges();
                }
            }
        }

        public static void DeleteByCodigo(string codigo)
        {
            using var db = new MinimarketContext();
            var producto = db.Productos.FirstOrDefault(p => p.Code == codigo);
            if (producto != null)
            {
                var nombre = producto.Name;
                db.Productos.Remove(producto);
                db.SaveChanges();

                // Registrar en auditoría
                db.Auditoria.Add(new AuditLog
                {
                    UserId = Session.CurrentUser?.Id ?? 1,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventDeleteProduct,
                    Details = $"Código: {codigo} - Nombre: {nombre}"
                });
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
        
        public static List<Minimarket.DTOs.ProductListDto> GetProductosBajoStock()
        {
            using var db = new MinimarketContext();
            return db.Productos
                .Where(p => p.IsActive && p.Stock <= p.MinStock)
                .OrderBy(p => p.Stock)
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
                })
                .ToList();
        }
    }
}
