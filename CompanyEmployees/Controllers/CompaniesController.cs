using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contracts;
using LoggerService;
using Entities.DataTransferObjects;
using AutoMapper;
namespace CompanyEmployees.Controllers
{
     [Route("api/Companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        
        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            
            
                var companies = _repository.Company.GetAllCompanies(trackChanges : false);
                var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
                return Ok(companiesDto);
                
                //throw new Exception ("Exception");
        }

        [HttpGet("{id}")]
        public IActionResult GetCompany(Guid id)
        {

            var company = _repository.Company.GetCompany(id, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn´t exists in the database");
                return NotFound();
            }

            else
            {

                
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }

        }
    }
}