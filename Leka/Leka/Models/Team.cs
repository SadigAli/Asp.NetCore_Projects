using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Leka.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required,StringLength(20)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Lastname { get; set; }
        [StringLength(100)]
        public string Image { get; set; }
        [Required, StringLength(50)]
        public string Profession { get; set; }
        public List<SosialIcon> SosialIcons { get; set; }
        [NotMapped]
        public List<string> Sosials { get; set; }
        [NotMapped]
        public List<string> Links { get; set; }
        [NotMapped]
        public List<int> SosialIconIds { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
