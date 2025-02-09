using MyShop.Models;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
    public interface IOrderDetailService : IBaseService<OrderDetail>
    {
        public void Update(OrderDetail orderDetail);
        public void UpdateDetails();

    }
}
