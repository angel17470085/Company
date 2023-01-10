using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contracts;
using LoggerService;
using Entities.DataTransferObjects;
using AutoMapper;
using Entities.Models;
using CompanyEmployees.ModelBinders;

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

        [HttpGet("{id}", Name = "CompanyById")]
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

        [HttpGet("collection/({ids})", Name ="CompanyCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("parameter ids is null");
            }

            var companyEntities = _repository.Company.GetByIds(ids, trackChanges:false);

            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("some ids are not valid in a collection");
                return NotFound();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            return Ok(companiesToReturn);
        }

       

        [HttpPost]
        public IActionResult CreateCompany ([FromBody]CompanyForCreationDto company)
        {
            if (company == null)
            {
                _logger.LogError($"ComopanyForCreationDto object set form client is null");
                return BadRequest("CompanyForCreationDto object is null");
            }

            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            _repository.Save();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new {id = companyToReturn.Id}, companyToReturn); // the second parameter is an anonymous type...

        }

        [HttpPost("collection")]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");

            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            _repository.Save();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }
    }
}