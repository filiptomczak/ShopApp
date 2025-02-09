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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private AppDbContext _db;
        public CompanyRepository(AppDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Company company)
        {
            _db.Companies.Update(company);
        }
    }
}
