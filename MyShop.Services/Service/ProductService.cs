using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.DataAcces.IRepositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.Service
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;
        public ProductService(
            IProductRepository repository,
            ICategoryService categoryService,
            IUnitOfWork unitOfWork,
            IFileService fileService
            ) : base(repository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
            _fileService = fileService; 
        }

        public void DeleteFile(string imageUrl)
        {
            _fileService.Delete(imageUrl);
        }

        public string GetFileStatus(ProductVM productVM, IFormFile file)
        {
            string fileName = _fileService.SaveFile(productVM, file);
            if(!string.IsNullOrEmpty(fileName))
            { 
                productVM.Product.ImageUrl = fileName;
            }
            if (productVM.Product.Id == 0)
            {
                _unitOfWork.ProductRepository.Add(productVM.Product);
                return "Product created";
            }
            else
            {
                _unitOfWork.ProductRepository.Update(productVM.Product);
                return "Product updated";
            }
        }

        public ProductVM UpdateVM(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _categoryService
                .GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });

            var productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = CategoryList,
            };
            //create
            if (id == null || id == 0)
            {
                return productVM;
            }
            //update
            else
            {
                productVM.Product = _repository.Get(p => p.Id == id);
                return productVM;
            }
        }

        
        
    }
}
