using Minimarket.DTOs;
using Models;

namespace Minimarket.Data
{
    public static class PurchaseRepository
    {
        public static void AgregarItem(int compraId, int productId, int cantidad, decimal costo)
        {
            using var db = new MinimarketContext();
            var compra = db.Compras.FirstOrDefault(c => c.Id == compraId);
            if (compra == null) throw new Exception("Compra no encontrada");
            var producto = db.Productos.FirstOrDefault(p => p.Id == productId);
            if (producto == null) throw new Exception("Producto no encontrado");
            var item = new PurchaseItem
            {
                PurchaseId = compraId,
                ProductId = productId,
                Qty = cantidad,
                Cost = costo,
                Subtotal = cantidad * costo
            };
            db.DetalleCompras.Add(item);
            producto.Stock += cantidad;
            producto.Cost = costo;
            db.MovimientosInventario.Add(new InventoryMovement
            {
                ProductId = productId,
                DateTime = DateTime.Now,
                Type = "IN",
                Qty = cantidad,
                Reason = "PURCHASE",
                RefId = compraId
            });
            db.SaveChanges();
            // Actualizar total de compra
            compra.Total = db.DetalleCompras.Where(x => x.PurchaseId == compraId).Sum(x => x.Subtotal);
            db.SaveChanges();
        }

        // Elimina un ítem de una compra existente y actualiza stock y total
        public static void EliminarItem(int itemId)
        {
            using var db = new MinimarketContext();
            var item = db.DetalleCompras.FirstOrDefault(x => x.Id == itemId);
            if (item == null) throw new Exception("Ítem no encontrado");
            var producto = db.Productos.FirstOrDefault(p => p.Id == item.ProductId);
            if (producto != null)
            {
                producto.Stock -= item.Qty;
            }
            int compraId = item.PurchaseId;
            db.DetalleCompras.Remove(item);
            db.SaveChanges();
            // Actualizar total de compra
            var compra = db.Compras.FirstOrDefault(c => c.Id == compraId);
            if (compra != null)
            {
                compra.Total = db.DetalleCompras.Where(x => x.PurchaseId == compraId).Sum(x => x.Subtotal);
                db.SaveChanges();
            }
        }

        public static Purchase GetByIdWithItems(int id)
        {
            using var db = new MinimarketContext();
            return db.Compras
                .Where(c => c.Id == id)
                .Select(c => new Purchase
                {
                    Id = c.Id,
                    SupplierId = c.SupplierId,
                    Date = c.Date,
                    DocNumber = c.DocNumber,
                    Total = c.Total,
                    PurchaseItems = c.PurchaseItems.Select(it => new PurchaseItem
                    {
                        Id = it.Id,
                        ProductId = it.ProductId,
                        Qty = it.Qty,
                        Cost = it.Cost,
                        Subtotal = it.Subtotal
                    }).ToList()
                })
                .FirstOrDefault();
        }

        public static List<PurchaseListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.Compras
                .Select(c => new PurchaseListDto
                {
                    Id = c.Id,
                    SupplierId = c.SupplierId,
                    SupplierName = c.Supplier.Name,
                    Date = c.Date,
                    NroDoc = c.DocNumber,
                    Total = c.Total
                })
                .ToList();
        }

        public static int CrearCompra(Purchase compra, bool registrarEgresoCaja = false)
        {
            using var db = new MinimarketContext();
            using var tx = db.Database.BeginTransaction();
            try
            {
                db.Compras.Add(compra);
                db.SaveChanges();

                // Registrar movimientos de inventario por cada ítem
                foreach (var it in compra.PurchaseItems)
                {
                    var producto = db.Productos.FirstOrDefault(p => p.Id == it.ProductId);
                    if (producto != null)
                    {
                        producto.Stock += it.Qty;
                        db.MovimientosInventario.Add(new InventoryMovement
                        {
                            ProductId = producto.Id,
                            DateTime = DateTime.Now,
                            Type = "Ingreso",
                            Qty = it.Qty,
                            Reason = "Compra",
                            RefId = compra.Id
                        });
                    }
                }
                db.SaveChanges();

                // Registrar egreso de caja si corresponde
                if (registrarEgresoCaja)
                {
                    db.MovimientosCaja.Add(new CashMovement
                    {
                        DateTime = DateTime.Now,
                        Type = Constants.CashMovementTypeOut,
                        Amount = compra.Total,
                        Concept = "Compra",
                        UserId = Session.CurrentUser?.Id ?? 1, // Usuario actual o 1 por defecto
                        SaleId = null
                    });
                    db.SaveChanges();
                }

                // Registrar en auditoría
                var proveedor = db.Proveedores.FirstOrDefault(p => p.Id == compra.SupplierId);
                db.Auditoria.Add(new AuditLog
                {
                    UserId = Session.CurrentUser?.Id ?? 1,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventCreatePurchase,
                    Details = $"Compra N° {compra.Id} - Proveedor: {proveedor?.Name ?? "N/A"} - Total: {compra.Total:C2}"
                });
                db.SaveChanges();

                tx.Commit();
                return compra.Id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
