using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;

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

        [HttpGet("{id}",Name = "GetEmployeeForCompany")]
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

        [HttpPost]
        public IActionResult CreateEmployeeForCompany (Guid companyId, [FromBody]EmployeeForCreationDto employee)
        {

            if (employee == null)
            {
               _logger.LogError("EmployeeForCreationDto object sent from client is null"); 

               return BadRequest("EmployeeForCreationDto object is null");
            }

            var company =_repository.Company.GetCompany(companyId, trackChanges: false); 
            if (company == null)
            {
                _logger.LogError($"company with id {companyId} doesnt exist in the database");

                return NotFound();
            }
            var employeeEntitie = _mapper.Map<Employee>(employee);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntitie);
            _repository.Save();
            
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntitie);

            return CreatedAtRoute("GetEmployeeForCompany", new {companyId, id = employeeToReturn.Id}, employeeToReturn);

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployeeForCompany ( Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"company with id : {companyId} does´nt exist in the database."); 
                return NotFound();
            }

            var employeeForCompany = _repository.Employee.GetEmployee(companyId,id, trackChanges: false);

            if (employeeForCompany == null)
            {
                _logger.LogInfo($"Employee with Id: {id} does´nt exists in the database"); 
                NotFound();
            }

            _repository.Employee.DeleteEmployee(employeeForCompany);
            _repository.Save();
            
            return NoContent();
            
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployeeForCompany (Guid companyId, Guid Id, [FromBody] EmployeeForUpdateDto employee)
        {
                if(employee == null)
                {
                    _logger.LogError("EmployeeForUpdateDto object sent from client is null");
                    return BadRequest("EmployeeForUpdateDto object is null");
                }

             var company = _repository.Company.GetCompany(companyId, trackChanges: false);
               
               if(company == null)
               {
                    _logger.LogInfo($"Company with id: {companyId} does´nt exist in the database.");
                    return NotFound();
               }

             var employeeEntitie = _repository.Employee.GetEmployee(companyId, Id, trackChanges: true);
               if(employeeEntitie == null)
               {
                _logger.LogInfo($"Employee with id: {Id} does´nt exist in the database.");
                return  NotFound();
               }
            
            _mapper.Map(employee, employeeEntitie);
            _repository.Save();
               return NoContent();

        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
         [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                _logger.LogError("patchDoc object set from client is null");
                return BadRequest("patchDoc object is null");
            }

            var company = _repository.Company.GetCompany(companyId, trackChanges: false);

            if (company == null)
            {
                 _logger.LogInfo($"company with Id {companyId} doesn't exist in the database");
                 return NotFound();
            }

            var employeeEntitie = _repository.Employee.GetEmployee(companyId, id, trackChanges: true);

            if (employeeEntitie == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database"); 
                return NotFound();
            }

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntitie);
            patchDoc.ApplyTo(employeeToPatch);
            _mapper.Map(employeeToPatch, employeeEntitie);

            _repository.Save();
            return NoContent();

        }
    }
}