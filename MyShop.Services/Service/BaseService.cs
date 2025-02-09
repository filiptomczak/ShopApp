using MyShop.DataAcces.IRepositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.Service
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        
        private readonly IRepository<T> _repository;
        public BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }
        public void Add(T entity)
        {
            _repository.Add(entity);
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _repository.DeleteRange(entities);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            return _repository.Get(filter, includeProperties, tracked);
        }

        public List<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            return _repository.GetAll(filter, includeProperties);
        }
    }
}
