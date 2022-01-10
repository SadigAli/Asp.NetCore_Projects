using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Leka.ViewModels;

namespace Leka.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
