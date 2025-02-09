using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class UserController : Controller
    {
        private readonly IAppUserService _appUserService;
        public UserVM userVM { get; set; }
        public UserController(IAppUserService appUserService)
        {
            _appUserService= appUserService;
        }
        public IActionResult Index()
        {            
            return View();
        }

        public IActionResult RoleManagement(string userId) 
        {
            var userVM = _appUserService.GetUserVM(userId,"Company");
            return View(userVM);
        }
        [HttpPost]
        public IActionResult RoleManagement(UserVM userVM)
        {
            _appUserService.SetNewRole(userVM);
            return RedirectToAction(nameof(Index));
        }
        #region API CALLS
        public IActionResult GetAll()
        {
            var users = _appUserService.GetAll();
            return Json(new { data = users },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string userId)
        {
            var user = _appUserService.GetById(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }
            _appUserService.SetLockoutDate(user);
            return Json(new { success = true, message = "Action successfuly taken" });
        }
        #endregion
    }
}
