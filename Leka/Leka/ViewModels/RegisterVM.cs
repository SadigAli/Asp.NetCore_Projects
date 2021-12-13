using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.ViewModels
{
    public class RegisterVM
    {
        [Required, StringLength(50)]
        public string Fullname { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, StringLength(50)]
        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
