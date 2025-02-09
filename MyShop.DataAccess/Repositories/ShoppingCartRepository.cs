using MyShop.Data;
using MyShop.DataAcces.Repositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private AppDbContext _db;
        public ShoppingCartRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ShoppingCart item)
        {
            _db.ShoppingCarts.Update(item);
        }
    }
}
