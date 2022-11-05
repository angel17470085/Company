using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contracts;
using LoggerService;
namespace CompanyEmployees.Controllers
{
     [Route("api/Companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        
        
        public CompaniesController(IRepositoryManager repository, ILoggerManager logger)
        {
            _logger = logger;
            _repository = repository;
            
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            try
            {
                var companies = _repository.Company.GetAllCompanies(trackChanges : false);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                
                _logger.LogError($"Something went wrong in the {nameof(GetCompanies)} action {ex}"); 

                return StatusCode(500, "Internal server error"); 
            }
        }
    }
}