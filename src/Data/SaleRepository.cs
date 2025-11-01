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

                // Actualizar stock y registrar movimientos de inventario
                foreach (var it in v.SaleItems)
                {
                    var producto = db.Productos.FirstOrDefault(p => p.Id == it.ProductId);
                    if (producto != null)
                    {
                        producto.Stock -= it.Qty;
                        db.MovimientosInventario.Add(new InventoryMovement
                        {
                            ProductId = producto.Id,
                            DateTime = DateTime.Now,
                            Type = "OUT",
                            Qty = it.Qty,
                            Reason = "SALE",
                            RefId = v.Id
                        });
                    }
                }
                db.SaveChanges();

                // Registrar movimiento de caja (ingreso por venta)
                db.MovimientosCaja.Add(new CashMovement
                {
                    DateTime = DateTime.Now,
                    Type = "IN",
                    Amount = v.Total,
                    Concept = "Venta",
                    UserId = v.UserId,
                    SaleId = v.Id
                });
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
