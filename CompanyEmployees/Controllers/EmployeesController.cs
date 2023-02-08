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
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges : false);

            if (company == null)
            {
                _logger.LogInfo($"company with {companyId} does't exists in the database");
                return NotFound();
            }
            else 
            {

                var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges: false);

                var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
                return Ok (employeesDto);
            }
        }

        [HttpGet("{id}",Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges : false);

            if (company == null)
            {
                _logger.LogInfo($"the company with id {companyId} does'nt exists");
                return NotFound();
            }

            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);

            if (employeeDb == null)
            {
                _logger.LogInfo($"Employee with Id {id} does'nt exist in the database");
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employeeDb);
            return Ok(employeeDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany (Guid companyId, [FromBody]EmployeeForCreationDto employee)
        {

            if (employee == null)
            {
               _logger.LogError("EmployeeForCreationDto object sent from client is null"); 

               return BadRequest("EmployeeForCreationDto object is null");
            }

             if (!ModelState.IsValid)
             {
                _logger.LogError("invalid model state for the EmployeeForCreationDto object");
                return UnprocessableEntity(ModelState); 
             }   

            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false); 
            if (company == null)
            {
                _logger.LogError($"company with id {companyId} doesnt exist in the database");

                return NotFound();
            }
            var employeeEntitie = _mapper.Map<Employee>(employee);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntitie);
           await _repository.SaveAsync();
            
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntitie);

            return CreatedAtRoute("GetEmployeeForCompany", new {companyId, id = employeeToReturn.Id}, employeeToReturn);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeForCompany ( Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"company with id : {companyId} does´nt exist in the database."); 
                return NotFound();
            }

            var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId,id, trackChanges: false);

            if (employeeForCompany == null)
            {
                _logger.LogInfo($"Employee with Id: {id} does´nt exists in the database"); 
                NotFound();
            }

            _repository.Employee.DeleteEmployee(employeeForCompany);
           await _repository.SaveAsync();
            
            return NoContent();
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeForCompany (Guid companyId, Guid Id, [FromBody] EmployeeForUpdateDto employee)
        {
                if(employee == null)
                {
                    _logger.LogError("EmployeeForUpdateDto object sent from client is null");
                    return BadRequest("EmployeeForUpdateDto object is null");
                }
                 if (!ModelState.IsValid)
                    {
                        _logger.LogError("invalid model state for the EmployeeForUpdateDto object");
                        return UnprocessableEntity(ModelState); 
                    }   

             var company =await  _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
               
               if(company == null)
               {
                    _logger.LogInfo($"Company with id: {companyId} does´nt exist in the database.");
                    return NotFound();
               }

             var employeeEntitie =await _repository.Employee.GetEmployeeAsync(companyId, Id, trackChanges: true);
               if(employeeEntitie == null)
               {
                _logger.LogInfo($"Employee with id: {Id} does´nt exist in the database.");
                return  NotFound();
               }
            
            _mapper.Map(employee, employeeEntitie);
            await _repository.SaveAsync();
               return NoContent();

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
         [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                _logger.LogError("patchDoc object set from client is null");
                return BadRequest("patchDoc object is null");
            }

            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);

            if (company == null)
            {
                 _logger.LogInfo($"company with Id {companyId} doesn't exist in the database");
                 return NotFound();
            }

            var employeeEntitie = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);

            if (employeeEntitie == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database"); 
                return NotFound();
            }

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntitie);
            patchDoc.ApplyTo(employeeToPatch, ModelState);

            TryValidateModel(employeeToPatch);

              if (!ModelState.IsValid)
            {
                _logger.LogError("invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeEntitie);
            await _repository.SaveAsync();
            return NoContent();

        }
    }
}