using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Leka.ViewModels;

namespace Leka.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        public string Name { get; set; }
        [Required, StringLength(10)]
        public string Code { get; set; }
        public List<ProductColor> ProductColors { get; set; }
    }
}
