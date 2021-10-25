using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required,StringLength(20)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Lastname { get; set; }
        [Required, StringLength(50)]
        public string Profession { get; set; }
        public SosialIcon SosialIcon { get; set; }
        public int SosialIconId { get; set; }

    }
}
