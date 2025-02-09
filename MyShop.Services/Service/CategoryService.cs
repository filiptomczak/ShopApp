using MyShop.DataAcces.IRepositories;
using MyShop.DataAccess.IRepositories;
using MyShop.Models;
using MyShop.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.Service
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository repository,
            IUnitOfWork unitOfWork)
            : base(repository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public void Delete(int? id)
        {
            var cat = Get(x=> x.Id == id);
            if (cat != null)
            {
                _repository.Delete(cat);
            }
        }
        public void Update(Category category) { 
            _repository.Update(category);
        }
    }
}
