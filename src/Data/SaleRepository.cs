using Microsoft.EntityFrameworkCore;
using Models;

namespace Minimarket.Data
{
    public static class SaleRepository
    {
        public static int CrearVenta(Sale v)
        {
            using var db = new MinimarketContext();
            using var tx = db.Database.BeginTransaction();
            try
            {
                // Agregar la venta y sus ítems
                db.Ventas.Add(v);
                db.SaveChanges();

                // Actualizar stock de cada producto vendido
                foreach (var it in v.SaleItems)
                {
                    var producto = db.Productos.FirstOrDefault(p => p.Id == it.ProductId);
                    if (producto != null)
                    {
                        producto.Stock -= it.Qty;
                    }
                }
                db.SaveChanges();

                tx.Commit();
                return v.Id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static List<Sale> ReporteVentasPorFecha(DateTime desde, DateTime hasta)
        {
            using var db = new MinimarketContext();
            return db.Ventas
                .Where(v => v.DateTime >= desde && v.DateTime <= hasta)
                .OrderByDescending(v => v.DateTime)
                .Include(v => v.SaleItems)
                .ToList();
        }
    }
}
