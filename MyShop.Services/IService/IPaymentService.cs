using MyShop.Models;
using MyShop.Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface IPaymentService
    {
        public Session HandlePayment(ShoppingCartVM shoppingCartVM);
        public Refund HandleRefund(OrderHeader order);

    }
}
