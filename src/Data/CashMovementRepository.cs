using Minimarket.DTOs;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace Minimarket.Data
{
    public static class CashMovementRepository
    {
        public static List<CashMovementListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.MovimientosCaja
                .OrderByDescending(x => x.DateTime)
                .Select(x => new CashMovementListDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Amount = x.Amount,
                    Concept = x.Concept,
                    User = x.User != null ? x.User.Username : "N/A",
                    DateTime = x.DateTime
                })
                .ToList();
        }

        public static void Create(CashMovement movement)
        {
            using var db = new MinimarketContext();
            db.MovimientosCaja.Add(movement);
            db.SaveChanges();
        }

        public static decimal GetSaldoCaja()
        {
            using var db = new MinimarketContext();
            var ingresos = db.MovimientosCaja.Where(m => m.Type == Constants.CashMovementTypeIn).Sum(m => (decimal?)m.Amount) ?? 0;
            var egresos = db.MovimientosCaja.Where(m => m.Type == Constants.CashMovementTypeOut).Sum(m => (decimal?)m.Amount) ?? 0;
            return ingresos - egresos;
        }
    }
}
