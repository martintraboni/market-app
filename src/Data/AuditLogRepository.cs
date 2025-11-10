using Minimarket.DTOs;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Minimarket.Data
{
    public static class AuditLogRepository
    {
        public static List<AuditLogListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.Auditoria
                .Include(a => a.User)
                .OrderByDescending(a => a.DateTime)
                .Select(a => new AuditLogListDto
                {
                    Id = a.Id,
                    DateTime = a.DateTime,
                    User = a.User.Username,
                    Event = a.Event,
                    Details = a.Details
                })
                .ToList();
        }

        public static List<AuditLogListDto> GetByDateRange(DateTime desde, DateTime hasta)
        {
            using var db = new MinimarketContext();
            return db.Auditoria
                .Include(a => a.User)
                .Where(a => a.DateTime >= desde && a.DateTime <= hasta)
                .OrderByDescending(a => a.DateTime)
                .Select(a => new AuditLogListDto
                {
                    Id = a.Id,
                    DateTime = a.DateTime,
                    User = a.User.Username,
                    Event = a.Event,
                    Details = a.Details
                })
                .ToList();
        }
    }
}
