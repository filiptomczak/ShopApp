using Microsoft.AspNetCore.Http;
using MyShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface IFileService
    {
        public string SaveFile(ProductVM productVM, IFormFile file);
        public void Delete(string filePath);
    }
}
