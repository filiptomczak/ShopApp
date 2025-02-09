using MyShop.DataAcces.Repositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Models;

namespace MyShop.DataAcces.IRepositories
{
    public interface IAppUserRepository:IRepository<AppUser>
    {
        public void Update(AppUser appUser);
    }
}
