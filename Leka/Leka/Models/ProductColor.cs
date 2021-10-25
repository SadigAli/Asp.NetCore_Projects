using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class ProductColor
    {
        public int Id { get; set; }
        public Color Color { get; set; }
        public int ColorId { get; set; }
        public ProductImage ProductImage { get; set; }
        public int ProductImageId { get; set; }

    }
}
