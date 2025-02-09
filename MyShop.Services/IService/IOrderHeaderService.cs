using MyShop.Models;
using MyShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface IOrderHeaderService : IBaseService<OrderHeader>
    {
        void UpdateStatus(int id, string sessionId, string paymentIntentId=null);
        void UpdateStripePaymentId(int id, string status, string paymentStatus);
        OrderHeader UpdateOrderDetails(OrderVM order);
        void PrepareShipping(OrderVM order);
    }
}
