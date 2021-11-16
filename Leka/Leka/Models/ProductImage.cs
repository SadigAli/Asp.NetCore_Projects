using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Image { get; set; }
        public bool? PosterStatus { get; set; }
        public ProductColor ProductColor { get; set; }
        public int ProductColorId { get; set; }


    }
}
