using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository;
using Entities;
namespace CompanyEmployees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public IRepositoryManager _repository { get; }

        public WeatherForecastController(IRepositoryManager repository)
        {
            _repository = repository;
            

        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            
           return new string[] { "value1", "value2" };
        }
    }
}
