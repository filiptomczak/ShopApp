using MyShop.DataAccess.IRepositories;
using System.Linq.Expressions;

namespace MyShop.Services.IService
{
    public interface IBaseService<T>
    {
        List<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        //tracked = false prevents from EF default behaviour with automatically updating entities when saving
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}