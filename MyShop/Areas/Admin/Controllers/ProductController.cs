    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using MyShop.Utility;
using MyShop.Services.IService;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CONSTS.ROLE_ADMIN)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _service;
        private readonly ICategoryService _categoryService;
        public ProductController(
            IUnitOfWork unitOfWork,
            IProductService service,
            IWebHostEnvironment webHostEnvironment,
            ICategoryService categoryService
            )
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var products = _service.GetAll(includeProperties:"Category");
            
            return View(products);
        }
        public IActionResult Update(int? id)
        {          
            var productVM = _service.UpdateVM(id);
            return View(productVM);
            
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
        }
        [HttpPost]
        public IActionResult Update(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileStatus = _service.GetFileStatus(productVM, file);
                TempData["success"] = fileStatus;
                
                _unitOfWork.Save();
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _categoryService
                    .GetAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    });
                return View(productVM);
            }
        }
        /*public IActionResult Edit(int? id)
        { 
            if(id == null || id == 0)
            {
                return NotFound();
            }
            var product = _unitOfWork.ProductRepository.Get(p=>p.Id== id);
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository
                .GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            var productVM = new ProductVM()
            {
                Product = product,
                CategoryList = CategoryList,
            };
            return View(productVM); 
        }
        [HttpPost]
        public IActionResult Edit(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Category updated";

                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.CategoryRepository
                    .GetAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    });
                return View(productVM);
            }
        }*/

        //public IActionResult Delete(int? id) 
        //{
        //    if(id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var productToDelete = _unitOfWork.ProductRepository.Get(p => p.Id == id);
        //    _unitOfWork.ProductRepository.Delete(productToDelete);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted";

        //    return RedirectToAction("Index", "Product");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _service.GetAll(includeProperties: "Category");
            return Json(new { data = products }, 
                new JsonSerializerOptions { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _service.Get(p => p.Id == id);
            if (productToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _service.DeleteFile(productToDelete.ImageUrl);
            _service.Delete(productToDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });

        }
        #endregion
    }
}
