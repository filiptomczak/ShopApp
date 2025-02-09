using Microsoft.AspNetCore.Identity;
using MyShop.Models;
using MyShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface IAppUserService : IBaseService<AppUser>
    {
        AppUser GetById(string id, string inclProp=null);
        string GetRoleId(string userId);
        string GetUserRole(string userId);
        //string GetUserRoleName(string roleId);
        List<AppUser> GetAll();
        UserVM GetUserVM(string userId, string inclProp = null);
        void SetNewRole(UserVM userVM);
        void SetLockoutDate(AppUser user);
    }
}
