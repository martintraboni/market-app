using Models;

namespace Minimarket.Data
{
    public static class RoleRepository
    {
        public static List<Role> GetAll()
        {
            using var db = new MinimarketContext();
            return db.Roles.OrderBy(r => r.Id).ToList();
        }

        public static Role? GetById(int id)
        {
            using var db = new MinimarketContext();
            return db.Roles.FirstOrDefault(r => r.Id == id);
        }

        public static Role? GetByCode(string code)
        {
            using var db = new MinimarketContext();
            return db.Roles.FirstOrDefault(r => r.RoleCode == code);
        }
    }
}
