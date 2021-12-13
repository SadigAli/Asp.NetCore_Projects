using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class SosialIcon
    {
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string Name { get; set; }
        [Required, StringLength(150)]
        public string Link { get; set; }
        [Required, StringLength(50)]
        public string Icon { get; set; }
        public Team Team { get; set; }
        public int TeamId { get; set; }
    }
}
