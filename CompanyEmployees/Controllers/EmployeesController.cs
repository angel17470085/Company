using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Entities.DataTransferObjects;
namespace CompanyEmployees.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/employees")]
    public class EmployeesController : ControllerBase
    {
      
      
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        
        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
          
        }

        [HttpGet]
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges : false);

            if (company == null)
            {
                _logger.LogInfo($"company with {companyId} does't exists in the database");
                return NotFound();
            }
            else 
            {

                var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges: false);

                var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
                return Ok (employeesDto);
            }
        }
    }
}