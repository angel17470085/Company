using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Contracts;
using Entities;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Employee> GetEmployees(Guid companyId,  bool trackChanges)
        {
            return FindByCondition(c=> c.CompanyId.Equals(companyId), trackChanges).OrderBy(e=>e.Name);
        }

       public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            return FindByCondition(e=> e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefault();
        }
    }
}