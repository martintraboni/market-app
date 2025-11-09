using Minimarket.DTOs;
using Models;

namespace Minimarket.Data
{
    public static class PurchaseRepository
    {
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
                            Type = "IN",
                            Qty = it.Qty,
                            Reason = "PURCHASE",
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
                        Type = "OUT",
                        Amount = compra.Total,
                        Concept = "Compra",
                        UserId = compra.SupplierId,
                        SaleId = null
                    });
                    db.SaveChanges();
                }

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
