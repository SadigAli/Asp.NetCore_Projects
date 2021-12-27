using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Leka.Models.Enums;

namespace Leka.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string Fullname { get; set; }
        [Required, StringLength(100)]
        public string Email { get; set; }
        [Required, StringLength(20)]
        public string Phone { get; set; }
        [Required, StringLength(10)]
        public string ZipCode { get; set; }
        [Required, StringLength(150)]
        public string Address { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime Date { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public double TotalPrice { get; set; }
    }
}
