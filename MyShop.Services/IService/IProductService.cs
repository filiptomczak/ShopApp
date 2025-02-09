using Microsoft.AspNetCore.Http;
using MyShop.Models;
using MyShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface IProductService : IBaseService<Product>
    {
        public ProductVM UpdateVM(int? id);
        public string GetFileStatus(ProductVM productVM, IFormFile file);
        public void DeleteFile(string imageUrl);
    }
}
