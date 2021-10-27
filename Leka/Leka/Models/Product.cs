﻿using Leka.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required,StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; }
        [Required,Column(TypeName ="decimal(9,2)")]
        public double CostPrice { get; set; }
        [Required, Column(TypeName = "decimal(9,2)")]
        public double SalePrice { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public double DiscountPrice { get; set; }
        public bool StockStatus { get; set; } = true;
        
        public Gender Gender { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductTag> ProductTags { get; set; }
    }
}