using Microsoft.AspNetCore.Mvc;
using MyShop.DataAcces.IRepositories;
using MyShop.Utility;
using System.Security.Claims;

namespace MyShop.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(CONSTS.SESSION_CART) == null)
                {
                    HttpContext.Session.SetInt32(CONSTS.SESSION_CART,
                        _unitOfWork.ShoppingCartRepository.GetAll(x =>
                            x.AppUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(CONSTS.SESSION_CART));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
