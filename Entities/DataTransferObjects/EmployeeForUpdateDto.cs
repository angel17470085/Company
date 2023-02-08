using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Entities.DataTransferObjects
{
    public class EmployeeForUpdateDto : EmployeeForManipulation
    {

        
        public string Name { get; set; }
       
        public int Age  { get; set; }

        public string Position { get; set; }


    }
}