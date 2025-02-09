using MyShop.Data;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;

namespace MyShop.DataAcces.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppDbContext _db;
        public CategoryRepository(AppDbContext db) : base(db)
        {
            _db= db;
        }
        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
