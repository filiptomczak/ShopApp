using MyShop.Data;
using MyShop.DataAcces.Repositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppDbContext _db;
        public ProductRepository(AppDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
