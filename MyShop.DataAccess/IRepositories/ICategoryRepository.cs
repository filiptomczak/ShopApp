using MyShop.DataAccess.IRepositories;
using MyShop.Models;

namespace MyShop.DataAcces.IRepositories
{
    public interface ICategoryRepository:IRepository<Category>
    {
        void Update(Category category);
    }
}
