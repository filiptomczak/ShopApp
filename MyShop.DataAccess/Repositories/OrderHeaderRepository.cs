using MyShop.Data;
using MyShop.DataAcces.IRepositories;
using MyShop.DataAcces.Repositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private AppDbContext _db;
        public OrderHeaderRepository(AppDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var order = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    order.PaymentStatus = paymentStatus;
                }
            }
        }

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var order=_db.OrderHeaders.FirstOrDefault(x=>x.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                order.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                order.PaymentIntentId= paymentIntentId;
               // order.PaymnetDate = DateTime.Now;
            }
		}
	}
}
