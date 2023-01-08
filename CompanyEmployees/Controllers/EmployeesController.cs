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

        [HttpGet("{id}")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges : false);

            if (company == null)
            {
                _logger.LogInfo($"the company with id {companyId} does'nt exists");
                return NotFound();
            }

            var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);

            if (employeeDb == null)
            {
                _logger.LogInfo($"Employee with Id {id} does'nt exist in the database");
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employeeDb);
            return Ok(employeeDto);
        }

    }
}