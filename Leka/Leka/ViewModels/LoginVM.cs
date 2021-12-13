using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.ViewModels
{
    public class LoginVM
    {
        [Required,StringLength(50)]
        public string Username { get; set; }
        [Required,MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
