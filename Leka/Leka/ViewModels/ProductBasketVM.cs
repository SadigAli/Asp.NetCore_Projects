using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.ViewModels
{
    public class ProductBasketVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public double SalePrice { get; set; }
        public int Count { get; set; }
        public string Image { get; set; }
    }
}
