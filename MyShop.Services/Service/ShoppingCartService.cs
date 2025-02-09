using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAcces.IRepositories;
using MyShop.DataAccess.IRepositories;
using MyShop.DataAccess.Repositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using MyShop.Utility;
using System.Security.Claims;
using Stripe.Checkout;
using Azure;

namespace MyShop.Services.Service
{
    public class ShoppingCartService : BaseService<ShoppingCart>, IShoppingCartService
    {
        private readonly IShoppingCartRepository _repository;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartService(
            IShoppingCartRepository repository, 
            IHttpContextAccessor httpContextAccessor,
            IOrderHeaderService orderHeaderService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork)
                : base(repository)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _orderHeaderService = orderHeaderService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public void Minus(int cartId)
        {
            var cart = Get(s => s.Id == cartId);
            if (cart.Count == 1)
            {
                Delete(cart);
                _httpContextAccessor.HttpContext.Session
                    .SetInt32(CONSTS.SESSION_CART,
                    GetAll(x => x.AppUserId == cart.AppUserId).Count() - 1);
            }
            else
            {
                cart.Count--;
                Update(cart);
            }
        }

        public void Plus(int cartId)
        {
            var cart = Get(s => s.Id == cartId);
            cart.Count++;
            Update(cart);
        }

        public void Remove(int cartId)
        {
            var cart = Get(s => s.Id == cartId);
            Delete(cart);

            _httpContextAccessor.HttpContext.Session
                .SetInt32(CONSTS.SESSION_CART,
                GetAll(x => x.AppUserId == cart.AppUserId).Count() - 1);
        }

        public void Update(ShoppingCart cart)
        {
            _repository.Update(cart);
        }

        public double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
        public ShoppingCartVM GetShoppingCartVM()
        {
            var userId = GetUserId();
            var shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = GetAll(s => s.AppUserId == userId, includeProperties: "Product"),
                OrderHeader = new(),
            };
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return shoppingCartVM;
        }
        public ShoppingCartVM UpdateShoppingCartVM()
        {
            var userId = GetUserId();
            var ShoppingCartVM = GetShoppingCartVM();

            ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUserRepository.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.AppUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.AppUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.Street = ShoppingCartVM.OrderHeader.AppUser.Street;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.AppUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.AppUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.AppUser.PostalCode;

            return ShoppingCartVM;
        }

        public (ShoppingCartVM, string url) ProcessOrder(ShoppingCartVM shoppingCartVM)
        {
            //var ShoppingCartVM = GetShoppingCartVM();
            var userId = GetUserId();

            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.AppUserId = userId;

            //produces error -> app user is navigation property and new instance is being created
            //ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUserRepository.Get(u => u.Id == userId);
            var appUser = _unitOfWork.AppUserRepository.Get(u => u.Id == userId);

            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                //payment required
                shoppingCartVM.OrderHeader.PaymentStatus = CONSTS.PAYMENT_STATUS_PENDING;
                shoppingCartVM.OrderHeader.OrderStatus = CONSTS.STATUS_PENDING;
            }
            else
            {
                //company user
                shoppingCartVM.OrderHeader.PaymentStatus = CONSTS.PAYMENT_STATUS_DELAYED_PAYMENT;
                shoppingCartVM.OrderHeader.OrderStatus = CONSTS.STATUS_APPROVED;
            }

            _orderHeaderService.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            if (shoppingCartVM.ShoppingCartList == null) {
                SetShoppingCartList(userId, shoppingCartVM);
            }
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetailRepository.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                //payment logic
                var session = _paymentService.HandlePayment(shoppingCartVM);

                _orderHeaderService.UpdateStripePaymentId(
                    shoppingCartVM.OrderHeader.Id,
                    session.Id,
                    session.PaymentIntentId);
                _unitOfWork.Save();

                _httpContextAccessor.HttpContext.Session.Clear();
                //_httpContextAccessor.HttpContext.Response.Headers
                //    .Add("Location", session.Url);
                //return new StatusCodeResult(303);
                return (shoppingCartVM, session.Url);
            }
            return (shoppingCartVM, null);
        }
        public void HandlePayment(int id)
        {
            var orderHeader = _orderHeaderService.Get(x => x.Id == id, includeProperties: "AppUser");
            if (orderHeader.PaymentStatus != CONSTS.PAYMENT_STATUS_DELAYED_PAYMENT)
            {
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _orderHeaderService.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _orderHeaderService.UpdateStatus(id, CONSTS.STATUS_APPROVED, CONSTS.PAYMENT_STATUS_APPROVED);
                    _unitOfWork.Save();
                }
            }

            var shoppingCarts = GetAll(x => x.AppUserId == orderHeader.AppUserId);
            DeleteRange(shoppingCarts);
      
        }
        private void SetShoppingCartList(string userId, ShoppingCartVM shoppingCartVM)
        {
            shoppingCartVM.ShoppingCartList = GetAll(s => s.AppUserId == userId, includeProperties: "Product");

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
        }
        private string GetUserId()
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
