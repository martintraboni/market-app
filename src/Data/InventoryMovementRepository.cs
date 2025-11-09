using Minimarket.DTOs;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace Minimarket.Data
{
    public static class InventoryMovementRepository
    {
        public static List<InventoryMovementListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.MovimientosInventario
                .OrderByDescending(x => x.DateTime)
                .Select(x => new InventoryMovementListDto
                {
                    Id = x.Id,
                    Product = x.Product.Name,
                    Type = x.Type,
                    Qty = x.Qty,
                    Reason = x.Reason,
                    DateTime = x.DateTime
                })
                .ToList();
        }
    }
}
