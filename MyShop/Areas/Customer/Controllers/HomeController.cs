using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace MyShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.ProductRepository
                .Get(p => p.Id == productId, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }
            var shoppingCart = new ShoppingCart
                {
                    Product = product,
                    Count = 1,
                    ProductId = productId,
                };
            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart) 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.AppUserId = userId;
            var shoppingCartFromDb = _unitOfWork.ShoppingCartRepository
                .Get(s=>s.AppUserId == userId && s.ProductId == shoppingCart.ProductId);

            if (shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartRepository.Update(shoppingCartFromDb);
                _unitOfWork.Save();
            }
            else
              {
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(CONSTS.SESSION_CART,
                    _unitOfWork.ShoppingCartRepository.GetAll(x => x.AppUserId == userId).Count());
            }

            TempData["success"] = $"{shoppingCart.Count} added to Cart";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
