using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using MyShop.Utility;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartService _shoppingCartService;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(
            IUnitOfWork unitOfWork,
            IShoppingCartService shoppingCartService)
        {
            _unitOfWork = unitOfWork;
            _shoppingCartService = shoppingCartService;
        }
        public IActionResult Index()
        {        
            ShoppingCartVM = _shoppingCartService.GetShoppingCartVM();
            return View(ShoppingCartVM);
        }
        public IActionResult Plus(int CartId)
        {
            _shoppingCartService.Plus(CartId);
            _unitOfWork.Save();
            TempData["success"] = "plus one";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int CartId)
        {
            _shoppingCartService.Minus(CartId);
            _unitOfWork.Save();
            TempData["success"] = " minus one";
            return RedirectToAction(nameof(Index));


        }
        public IActionResult Remove(int CartId)
        {
            _shoppingCartService.Remove(CartId);
            _unitOfWork.Save();
            TempData["success"] = $"removed";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            ShoppingCartVM = _shoppingCartService.UpdateShoppingCartVM();
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            (ShoppingCartVM, string sessionUrl) = _shoppingCartService.ProcessOrder(ShoppingCartVM);
            if (!string.IsNullOrEmpty(sessionUrl))
            {
                Response.Headers.Add("Location", sessionUrl);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new {id=ShoppingCartVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
            _shoppingCartService.HandlePayment(id);
            _unitOfWork.Save();

            return View(id);
        }

        #region API CALLS
        public class CartRequest
        {
            public int CartId { get; set; }
            public string? Action { get; set; }
            public double OldTotal { get; set; }
        }
        [HttpPost]
        public IActionResult Update([FromBody] CartRequest cartRequest)
        {
            var cart = _unitOfWork.ShoppingCartRepository.Get(s => s.Id == cartRequest.CartId, includeProperties: "Product");
            if (cart == null)
                return Json(new { success = false });
            //get price from GetPriceBasedOnQuantity, cart.Price = 0 - not mapped
            var totalWithoutCurrentProduct = cartRequest.OldTotal - _shoppingCartService.GetPriceBasedOnQuantity(cart) * cart.Count;

            if (cartRequest.Action == "plus")
                cart.Count++;
            else if (cartRequest.Action == "minus")
            {
                cart.Count--;
                if (cart.Count == 0)
                {
                    _unitOfWork.ShoppingCartRepository.Delete(cart);
                    HttpContext.Session.SetInt32(CONSTS.SESSION_CART,
                        _unitOfWork.ShoppingCartRepository.GetAll(x => x.AppUserId == cart.AppUserId).Count() - 1);
                    _unitOfWork.Save();
                    return Json(new
                    {
                        success = true,
                        newQuantity = 0,
                        newPrice = 0,
                        newTotal = totalWithoutCurrentProduct.ToString("c"),
                    });
                }
            }
            else
                return Json(new { success = false });

            cart.Price = _shoppingCartService.GetPriceBasedOnQuantity(cart);
            var newTotal = totalWithoutCurrentProduct + cart.Price * cart.Count;            

            _unitOfWork.ShoppingCartRepository.Update(cart);
            _unitOfWork.Save();

            return Json(new
            {
                success = true,
                newQuantity = cart.Count,
                newPrice = cart.Price.ToString("c"),
                newTotal = newTotal.ToString("c"),
            });
        }
        #endregion
    }
}
  