using Models;

namespace MiniMarket.Data
{
    public static class CategoryRepository
    {

        public static List<Category> GetAll()
        {
            using var db = new MinimarketContext();
            return db.Categorias.ToList();
        }

        public static void Delete(int id)
        {
            using var db = new MinimarketContext();
            var categoria = db.Categorias.Find(id);
            if (categoria != null)
            {
                db.Categorias.Remove(categoria);
                db.SaveChanges();
            }
        }

        public static List<Minimarket.DTOs.CategoryListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.Categorias
                .Select(c => new Minimarket.DTOs.CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
        }
    }
}
