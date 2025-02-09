using MyShop.Services.IService;
using MyShop.Models;
using MyShop.DataAcces.IRepositories;

namespace MyShop.Services.Service
{
    public class OrderDetailService : BaseService<OrderDetail>, IOrderDetailService
    {
        private readonly IOrderDetailRepository _repository;
        public OrderDetailService(IOrderDetailRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public void Update(OrderDetail orderDetail)
        {
            _repository.Update(orderDetail);
        }

        public void UpdateDetails()
        {
            throw new NotImplementedException();
        }
    }
}
