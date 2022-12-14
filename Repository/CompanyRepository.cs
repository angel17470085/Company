using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Models;
namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base (repositoryContext)
        {
            
        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        {
             return FindAll(trackChanges).OrderBy(c=>c.Name).ToList();
        }

      public Company GetCompany(Guid companyId, bool trackChanges)
        {
        return FindByCondition(c=>c.Id.Equals(companyId), trackChanges).FirstOrDefault();
        }

        public void CreateCompany(Company company)
        {
           Create(company);
        }

        IEnumerable<Company> ICompanyRepository.GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
          return  FindByCondition(x=> ids.Contains(x.Id), trackChanges).ToList();
        }
    }
}