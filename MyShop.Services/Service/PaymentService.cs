using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.Service
{
    public class PaymentService : IPaymentService
    {
        public Session HandlePayment(ShoppingCartVM shoppingCartVM)
        {
            var domain = "https://localhost:7140/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cart/orderconfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart",

                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var item in shoppingCartVM.ShoppingCartList)
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
            return session;
        }

        public Refund HandleRefund(OrderHeader order)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = order.PaymentIntentId,
            };
            var service = new RefundService();
            var refund = service.Create(options);
            return refund;
        }
    }
}
