using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.IRepositories
{
    public interface ICompanyRepository:IRepository<Company>
    {
        public void Update(Company company);
    }
}
