using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShop.Data;
using MyShop.Models;
using MyShop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;
        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;   
        }
        public void Init()
        {
            //migrations
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error: {ex.Message}");
            }
            //create roles 
            if (!_roleManager.RoleExistsAsync(CONSTS.ROLE_ADMIN).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(CONSTS.ROLE_ADMIN)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(CONSTS.ROLE_CUSTOMER)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(CONSTS.ROLE_EMPLOYEE)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(CONSTS.ROLE_COMPANY)).GetAwaiter().GetResult();

                //create admin
                _userManager.CreateAsync(new AppUser
                {
                    UserName = "admin",
                    Email = "admin@wp.pl",
                    Name = "Admin",
                    PhoneNumber = "111111111",
                    Street = "Admin",
                    State = "Admin",
                    PostalCode = "00000",
                    City = "Admin",
                }, "Yapi@1234").GetAwaiter().GetResult();

                AppUser user = _db.AppUsers.FirstOrDefault(x => x.Email == "admin@wp.pl");
                _userManager.AddToRoleAsync(user, CONSTS.ROLE_ADMIN).GetAwaiter().GetResult();
            }
            return;            
        }
    }
}
