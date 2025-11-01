using Models;

namespace Data
{
    public static class CategoryRepository
    {
        public static List<Category> GetAll()
        {
            using var db = new MinimarketContext();
            return db.Categorias.ToList();
        }
    }
}
