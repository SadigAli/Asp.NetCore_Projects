using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class AppUser:IdentityUser
    {
        [Required,StringLength(50)]
        public string Fullname { get; set; }
        public bool IsAdmin { get; set; } = false;
        public List<Order> Orders { get; set; }
    }
}
