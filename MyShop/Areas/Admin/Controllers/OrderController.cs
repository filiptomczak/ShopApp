using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyShop.DataAcces.IRepositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using MyShop.Utility;
using Stripe;
using Stripe.Checkout;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text.Json;

namespace MyShop.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]

    public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrderHeaderService _headerService;
		private readonly IOrderDetailService _detailService;
        private readonly IPaymentService _paymentService;
		

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(
            IUnitOfWork unitOfWork, 
            IOrderHeaderService headerService, 
            IOrderDetailService detailService,
            IPaymentService paymentService )
        {
			_unitOfWork = unitOfWork;
            _headerService = headerService;
            _detailService = detailService;
            _paymentService = paymentService;
        }
        public IActionResult Index()
		{
			return View();
		}
        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderVM
            {
                OrderHeader = _headerService.Get(x => x.Id == orderId, includeProperties:"AppUser"),
                OrderDetail = _detailService.GetAll(x => x.OrderHeaderId == orderId,includeProperties:"Product"),
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles = CONSTS.ROLE_ADMIN+","+CONSTS.ROLE_EMPLOYEE)]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = _headerService.UpdateOrderDetails(OrderVM);
            _unitOfWork.Save();
            TempData["Success"] = "Order Deatils Updated!";

            return RedirectToAction(nameof(Details), new { orderId = orderFromDb.Id });
        }

        [Authorize(Roles = CONSTS.ROLE_ADMIN + "," + CONSTS.ROLE_EMPLOYEE)]
        public IActionResult StartProcessing()
        {
            _headerService.UpdateStatus(OrderVM.OrderHeader.Id, CONSTS.STATUS_IN_PROCESS);
            _unitOfWork.Save();

            TempData["Success"] = "Order Deatils Updated!";
            return RedirectToAction(nameof(Details), new {orderId=OrderVM.OrderHeader.Id});
        }
        [Authorize(Roles = CONSTS.ROLE_ADMIN + "," + CONSTS.ROLE_EMPLOYEE)]
        public IActionResult ShipOrder()
        {
            _headerService.PrepareShipping(OrderVM);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped!";
            return RedirectToAction(nameof(Details), new {orderId=OrderVM.OrderHeader.Id});
        }
        [Authorize(Roles = CONSTS.ROLE_ADMIN + "," + CONSTS.ROLE_EMPLOYEE)]
        public IActionResult CancelOrder()
        {
            var orderFromDb = _headerService.Get(x=>x.Id==OrderVM.OrderHeader.Id);

            if (orderFromDb.PaymentStatus == CONSTS.PAYMENT_STATUS_APPROVED)
            {                
                _paymentService.HandleRefund(orderFromDb);
                _headerService.UpdateStatus(orderFromDb.Id, CONSTS.STATUS_REFUNDED);
            }
            else
            {
                _headerService.UpdateStatus(orderFromDb.Id, CONSTS.STATUS_CANCELLED);
            }

            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled!";
            return RedirectToAction(nameof(Details), new {orderId=OrderVM.OrderHeader.Id});
        }
        [ActionName("Details")]
        [HttpPost]
        public IActionResult DelayedPayment()
        {
            OrderVM.OrderHeader =_unitOfWork.OrderHeaderRepository.Get(x=>x.Id==OrderVM.OrderHeader.Id,includeProperties:"AppUser");
            OrderVM.OrderDetail=_unitOfWork.OrderDetailRepository.GetAll(x=>x.OrderHeaderId==OrderVM.OrderHeader.Id,includeProperties:"Product");

            var domain = "https://localhost:7140/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/orderconfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",

                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            var session = service.Create(options);

            _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(
                OrderVM.OrderHeader.Id,
                session.Id,
                session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult OrderConfirmation(int orderHeaderId)
        {
            var orderHeader = _unitOfWork.OrderHeaderRepository.Get(x => x.Id == orderHeaderId);
            if (orderHeader.PaymentStatus != CONSTS.PAYMENT_STATUS_DELAYED_PAYMENT)
            {
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, CONSTS.PAYMENT_STATUS_APPROVED);
                    _unitOfWork.Save();
                }
            }
            HttpContext.Session.Clear();

            return View(orderHeaderId);
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll([FromQuery]string filter)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var headers = (User.IsInRole(CONSTS.ROLE_ADMIN) || User.IsInRole(CONSTS.ROLE_EMPLOYEE)) ?
                _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "AppUser").ToList() :
                _unitOfWork.OrderHeaderRepository.GetAll(x => x.AppUser.Id == userId, includeProperties: "AppUser").ToList();

            if (filter != null && filter?.ToLower() != "all")
            {
                headers = (filter == CONSTS.PAYMENT_STATUS_PENDING) ?
                    headers.Where(s => s.PaymentStatus == filter).ToList():
                    headers.Where(s => s.OrderStatus == filter).ToList();
            }
            return Json(new { data = headers },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
        #endregion  
    }
}
