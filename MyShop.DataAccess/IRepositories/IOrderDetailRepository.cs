using MyShop.DataAccess.IRepositories;
using MyShop.Models;

namespace MyShop.DataAcces.IRepositories
{
    public interface IOrderDetailRepository:IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
