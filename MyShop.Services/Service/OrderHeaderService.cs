using MyShop.DataAcces.IRepositories;
using MyShop.DataAccess.IRepositories;
using MyShop.DataAccess.Repositories;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Services.IService;
using MyShop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.Service
{
    public class OrderHeaderService : BaseService<OrderHeader>, IOrderHeaderService
    {
        private readonly IOrderHeaderRepository _repository;
        public OrderHeaderService(IOrderHeaderRepository repository):base(repository)
        {
            _repository = repository;
        }

        public void PrepareShipping(OrderVM order)
        {
            var orderFromDb = _repository.Get(x => x.Id == order.OrderHeader.Id);
            orderFromDb.Carrier = order.OrderHeader.Carrier;
            orderFromDb.TrackingNumber = order.OrderHeader.TrackingNumber;
            orderFromDb.OrderStatus = CONSTS.STATUS_SHIPPED;
            orderFromDb.ShipppingDate = DateTime.Now;
            if (orderFromDb.PaymentStatus == CONSTS.PAYMENT_STATUS_DELAYED_PAYMENT)
            {
                orderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _repository.Update(orderFromDb);
        }

        public OrderHeader UpdateOrderDetails(OrderVM order)
        {
            var orderFromDb = _repository.Get(x => x.Id == order.OrderHeader.Id);
            orderFromDb.Name = order.OrderHeader.Name;
            orderFromDb.PhoneNumber = order.OrderHeader.PhoneNumber;
            orderFromDb.Street = order.OrderHeader.Street;
            orderFromDb.City = order.OrderHeader.City;
            orderFromDb.State = order.OrderHeader.State;
            orderFromDb.PostalCode = order.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(order.OrderHeader.Carrier))
            {
                orderFromDb.Carrier = order.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(order.OrderHeader.TrackingNumber))
            {
                orderFromDb.TrackingNumber = order.OrderHeader.TrackingNumber;
            }            
            _repository.Update(orderFromDb);
            return orderFromDb;
        }

        public void UpdateStatus(int id, string sessionId, string paymentIntentId=null)
        {
            _repository.UpdateStatus(id, sessionId, paymentIntentId);
        }

        public void UpdateStripePaymentId(int id, string status, string paymentStatus)
        {
            _repository.UpdateStripePaymentId(id, status, paymentStatus);
        }
    }
}
