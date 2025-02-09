using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;
using MyShop.Services.IService;
using MyShop.Utility;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CONSTS.ROLE_ADMIN)]
    public class CategoryController : Controller
    {
        //private readonly AppDbContext _db;
        //private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        
        public CategoryController(
            IUnitOfWork unitOfWork, 
            ICategoryService categoryService)
        {
            //_repository = repository;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var categoryList = _categoryService.GetAll();
            return View(categoryList);
        }
        public IActionResult Create()
        {
            var category=new Category();
            return View(category);
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            //custom validation
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name and Display Order cannot be the same");
            }
            if (ModelState.IsValid)
            {
                _categoryService.Add(category);
                _unitOfWork.Save();

                TempData["success"] = "Category created";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _categoryService.Get(x => x.Id == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.Update(category);
                _unitOfWork.Save();

                TempData["success"] = "Category updated";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            _categoryService.Delete(id);
            _unitOfWork.Save();

            TempData["success"] = "Category deleted";
            return RedirectToAction("Index", "Category");
        }
    }
}
