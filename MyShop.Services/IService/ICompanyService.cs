using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface ICompanyService : IBaseService<Company>
    {
        public void Update(Company company);
    }
}
