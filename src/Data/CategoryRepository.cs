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
