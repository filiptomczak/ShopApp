using Microsoft.AspNetCore.Identity;
using MyShop.Data;
using MyShop.DataAcces.IRepositories;
using MyShop.DataAcces.Repositories;
using MyShop.DataAccess.IRepositories;

namespace MyShop.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;
        //private readonly Dictionary<Type, object> _repositories = new();
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public ICompanyRepository CompanyRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; private set; }
        public IAppUserRepository AppUserRepository { get; private set; }
        public IOrderDetailRepository OrderDetailRepository { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }

        public UnitOfWork(AppDbContext appDbContext,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ICompanyRepository companyRepository,
            IShoppingCartRepository shoppingCartRepository,
            IAppUserRepository appUserRepository,
            IOrderDetailRepository orderDetailRepository,
            IOrderHeaderRepository orderHeaderRepository)
        {
            _db = appDbContext;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            CompanyRepository = companyRepository;
            ShoppingCartRepository = shoppingCartRepository;
            AppUserRepository = appUserRepository;
            OrderDetailRepository = orderDetailRepository;
            OrderHeaderRepository = orderHeaderRepository;
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public List<IdentityUserRole<string>> GetUserRoles()
        {
            return _db.UserRoles.ToList();
        }
        public List<IdentityRole> GetRoles()
        {
            return _db.Roles.ToList();
        }

        //public IRepository<T> GetRepository<T>() where T : class
        //{
        //    return (IRepository<T>)_repositories[typeof(T)];
        //}
    }
}
