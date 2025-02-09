using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.Service
{

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
            
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void Delete(string filePath)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            var oldImgPath = Path.Combine(
                            wwwRootPath, filePath.TrimStart('/'));
            if (File.Exists(oldImgPath))
            {
                File.Delete(oldImgPath);
            }
        }

        public string SaveFile(ProductVM productVM, IFormFile file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");
                //do not update img if no new img is selected
                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    //delete old img
                    var oldImgPath = Path.Combine(
                        wwwRootPath, productVM.Product.ImageUrl.TrimStart('/'));
                    if (File.Exists(oldImgPath))
                    {
                        File.Delete(oldImgPath);
                    }
                }
                using (var fileStream = new FileStream(
                    Path.Combine(productPath, fileName),
                    FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return $"/images/product/{fileName}";
            }
            return string.Empty;
        }
    }
}
