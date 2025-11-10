using Minimarket.DTOs;

namespace Minimarket.Data
{
    public static class SupplierRepository
    {
        public static List<SupplierListDto> GetAll()
        {
            using var db = new MinimarketContext();
            return db.Proveedores
                .OrderByDescending(x => x.Id)
                .Select(s => new SupplierListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CUIT = s.CUIT,
                    Phone = s.Phone,
                    Email = s.Email
                })
                .ToList();
        }
    }
}
