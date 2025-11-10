using Microsoft.EntityFrameworkCore;
using Models;
using Minimarket.DTOs;

namespace Minimarket.Data
{
    public static class SaleRepository
    {
        public static List<SaleListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.Ventas
                .OrderByDescending(v => v.DateTime)
                .Select(v => new SaleListDto
                {
                    Id = v.Id,
                    User = v.User.Username,
                    PaymentMethod = v.PaymentMethod,
                    Total = v.Total,
                    DateTime = v.DateTime
                })
                .ToList();
        }

        public static Sale? GetByIdWithItems(int id)
        {
            using var db = new MinimarketContext();
            return db.Ventas
                .Include(v => v.User)
                .Include(v => v.SaleItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefault(v => v.Id == id);
        }

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
                    Type = Constants.CashMovementTypeIn,
                    Amount = v.Total,
                    Concept = "Venta",
                    UserId = v.UserId,
                    SaleId = v.Id
                });
                db.SaveChanges();

                // Registrar en auditoría
                db.Auditoria.Add(new AuditLog
                {
                    UserId = v.UserId,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventCreateSale,
                    Details = $"Venta N° {v.Id} - Método: {v.PaymentMethod} - Total: {v.Total:C2}"
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

        public static List<SaleListDto> GetReporteDtoByFecha(DateTime desde, DateTime hasta)
        {
            using var db = new MinimarketContext();
            return db.Ventas
                .Where(v => v.DateTime >= desde && v.DateTime <= hasta)
                .OrderByDescending(v => v.DateTime)
                .Select(v => new SaleListDto
                {
                    Id = v.Id,
                    DateTime = v.DateTime,
                    User = v.User.Username,
                    PaymentMethod = v.PaymentMethod,
                    Total = v.Total
                })
                .ToList();
        }
        
        public static void AnularVenta(int ventaId, string motivo)
        {
            using var db = new MinimarketContext();
            using var tx = db.Database.BeginTransaction();
            try
            {
                var venta = db.Ventas
                    .Include(v => v.SaleItems)
                    .ThenInclude(si => si.Product)
                    .FirstOrDefault(v => v.Id == ventaId);
                    
                if (venta == null)
                    throw new Exception("Venta no encontrada");
                
                // Restaurar stock de cada producto
                foreach (var item in venta.SaleItems)
                {
                    var producto = db.Productos.FirstOrDefault(p => p.Id == item.ProductId);
                    if (producto != null)
                    {
                        producto.Stock += item.Qty;
                        
                        // Registrar movimiento de inventario (reversión)
                        db.MovimientosInventario.Add(new InventoryMovement
                        {
                            ProductId = producto.Id,
                            DateTime = DateTime.Now,
                            Type = Constants.InventoryMovementTypeIngreso,
                            Qty = item.Qty,
                            Reason = $"Anulación venta N° {ventaId} - {motivo}",
                            RefId = ventaId
                        });
                    }
                }
                
                // Registrar movimiento de caja negativo (egreso por anulación)
                db.MovimientosCaja.Add(new CashMovement
                {
                    DateTime = DateTime.Now,
                    Type = Constants.CashMovementTypeOut,
                    Amount = venta.Total,
                    Concept = $"Anulación Venta N° {ventaId}",
                    UserId = Session.CurrentUser?.Id ?? 1
                });
                
                // Registrar en auditoría
                db.Auditoria.Add(new AuditLog
                {
                    UserId = Session.CurrentUser?.Id ?? 1,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventSaleCancellation,
                    Details = $"Venta N° {ventaId} - Total: {venta.Total:C2} - Motivo: {motivo}"
                });
                
                db.SaveChanges();
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
