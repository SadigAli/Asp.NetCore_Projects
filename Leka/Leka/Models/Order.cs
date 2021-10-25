using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(100)]
        public string Email { get; set; }
        [Required, StringLength(20)]
        public string Phone { get; set; }
        [Required, StringLength(150)]
        public string Address { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public OrderItem OrderItem { get; set; }
        public int OrderItemId { get; set; }
    }
}
