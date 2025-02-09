using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Models.ViewModels
{
    public class UserVM
    {
        public AppUser User{ get; set; }
        public IEnumerable<SelectListItem>? CompanyList { get; set; }
        public IEnumerable<SelectListItem>? RoleList { get; set; }
    }
}
