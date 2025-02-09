using MyShop.Data;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;

namespace MyShop.DataAcces.Repositories
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private AppDbContext _db;
        public AppUserRepository(AppDbContext db) : base(db)
        {
            _db= db;
        }

        public void Update(AppUser appUser)
        {
            _db.Update(appUser);
        }
    }
}
