using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Entities.DataTransferObjects
{
    public class UserForAuthenticationDto
    {

        [Required(ErrorMessage = "user name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password name is required")]
        public string Password { get; set; }

    }
}