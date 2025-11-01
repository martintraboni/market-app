using Models;

namespace Minimarket.Data
{
    public static class SupplierRepository
    {
        public static List<Minimarket.DTOs.SupplierListDto> GetAll()
        {
            using var db = new MinimarketContext();
            return db.Proveedores
                .Select(s => new Minimarket.DTOs.SupplierListDto
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
