using Microsoft.AspNetCore.Identity;
using MyShop.Data;
using MyShop.DataAccess.IRepositories;

namespace MyShop.DataAcces.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        ICompanyRepository CompanyRepository{ get; }
        IShoppingCartRepository ShoppingCartRepository{ get; }
        IAppUserRepository AppUserRepository{ get; }
        IOrderDetailRepository OrderDetailRepository{ get; }
        IOrderHeaderRepository OrderHeaderRepository{ get; }
        //IRepository<T> GetRepository<T>() where T : class;

        public void Save();
        public List<IdentityUserRole<string>> GetUserRoles();
        public List<IdentityRole> GetRoles();
        
    }
}
