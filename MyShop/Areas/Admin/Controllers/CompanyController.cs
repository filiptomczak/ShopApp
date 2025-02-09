using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using MyShop.Utility;
using System.Text.Json;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CONSTS.ROLE_ADMIN)]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(
            ICompanyService companyService, 
            IUnitOfWork unitOfWork)
        {
            _companyService = companyService;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var companies = _companyService.GetAll();
            return View(companies);
        }
        public IActionResult Update(int? id) 
        {
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            var company = _companyService.Get(c=>c.Id == id);
            return View(company);
        }
        [HttpPost]
        public IActionResult Update(Company company) 
        {
            if (ModelState.IsValid) {
                _companyService.Update(company);
                TempData["success"] = "Product updated";
                _unitOfWork.Save();
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(company);
            }
        }

        #region API CALLS
        public IActionResult GetAll()
        {
            var companies = _companyService.GetAll();
            return Json(new { data = companies }, 
                new JsonSerializerOptions { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        public IActionResult Delete(int? id)
        {
            var companyToDelete = _companyService.Get(c=>c.Id==id);
            if (companyToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _companyService.Delete(companyToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
