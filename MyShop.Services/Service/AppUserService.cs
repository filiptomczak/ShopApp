using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.DataAcces.IRepositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using MyShop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.Service
{
    public class AppUserService : BaseService<AppUser>, IAppUserService
    {
        private readonly IAppUserRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public AppUserService(IAppUserRepository repository,
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager
            ) : base(repository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public AppUser GetById(string id,string prop=null)
        {
            return _repository.Get(x => x.Id == id, includeProperties: prop);
        }

        public string GetRoleId(string userId)
        {
            return GetUserRoles().FirstOrDefault(x=> x.UserId == userId)?.RoleId;
        }

        public string GetUserRole(string userId)
        {
            var user = GetById(userId);
            var roleId = GetRoleId(userId);
            var roles = GetRoles();

            user.Role = roleId == null ?
                "" :
                roles.FirstOrDefault(x => x.Id == roleId).Id;

            return user.Role;
        }
        
        public UserVM GetUserVM(string userId, string prop)
        {
            var user = GetById(userId, prop);
            var roleId = GetRoleId(userId);
            var roles = GetRoles();
            var roleList = roles.Select(x => new SelectListItem
            {
                Text = roles.FirstOrDefault(r => r.Id == x.Id).Name,
                Value = x.Name,
            });
            var companyList = _unitOfWork.CompanyRepository
                .GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                });

            var userVM = new UserVM()
            {
                User = user,
                RoleList = roleList,
                CompanyList = companyList,
            };
            userVM.User.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
            return userVM;
        }

        public void SetNewRole(UserVM userVM)
        {
            var roleId = GetRoleId(userVM.User.Id);
            var oldRole = GetUserRoleName(roleId);


            var user = GetById(userVM.User.Id);
            if (userVM.User.Role == CONSTS.ROLE_COMPANY)
            {
                user.CompanyId = userVM.User.CompanyId;
            }
            if (oldRole != CONSTS.ROLE_COMPANY && userVM.User.Role == CONSTS.ROLE_COMPANY)
            {
                user.CompanyId = null;
            }
            _repository.Update(user);
            _unitOfWork.Save();

            if (userVM.User.Role != oldRole)
            {
                var userFromManager = _userManager.FindByIdAsync(userVM.User.Id).GetAwaiter().GetResult();
                _userManager.RemoveFromRoleAsync(userFromManager, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(userFromManager, userVM.User.Role).GetAwaiter().GetResult();
            }
        }

        private string GetUserRoleName(string roleId)
        {
            return GetRoles()?.FirstOrDefault(x => x.Id == roleId)?.Name;
        }
        private List<IdentityRole> GetRoles()
        {
            return _unitOfWork.GetRoles(); ;
        }

        private List<IdentityUserRole<string>> GetUserRoles()
        {
            return _unitOfWork.GetUserRoles();
        }

        public List<AppUser> GetAll()
        {
            var users = _repository.GetAll(includeProperties: "Company");
            var userRoles = GetUserRoles();
            var roles = GetRoles();


            foreach (var user in users)
            {
                //var roleId = GetRoleId(user.Id);
                user.Role = GetUserRole(user.Id);
                    //roleId == null ?
                    //"" :
                    //roles.FirstOrDefault(x => x.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }
            return users;
        }

        public void SetLockoutDate(AppUser user)
        {
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(99);
            }
            _repository.Update(user);
            _unitOfWork.Save();
        }
    }
}
