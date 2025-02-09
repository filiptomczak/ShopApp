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
    public class CompanyService : BaseService<Company>, ICompanyService
    {
        private readonly ICompanyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(
            ICompanyRepository repository, 
            IUnitOfWork unitOfWork) 
            : base(repository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void Update(Company company)
        {
            _repository.Update(company);    
        }
 
    }
}
