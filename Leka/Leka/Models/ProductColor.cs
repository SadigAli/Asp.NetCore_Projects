using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class ProductColor
    {
        public int Id { get; set; }
        public Color Color { get; set; }
        public int ColorId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public List<ProductImage> ProductImages { get; set; }

        [NotMapped]
        public IFormFile PosterImage { get; set; }
        [NotMapped]
        public IFormFile HoverImage { get; set; }
        [NotMapped]
        public List<IFormFile> Images { get; set; }
        [NotMapped]
        public List<int> ImageIds { get; set; }

    }
}
