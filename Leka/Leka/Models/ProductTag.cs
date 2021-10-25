using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class ProductTag
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}
