using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public List<ProductTag> ProductTags { get; set; }
    }
}
