using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface ICategoryService : IBaseService<Category>
    {
        public void Delete(int? id);
        public void Update(Category category);
    }
}
