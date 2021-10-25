using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class AboutSkill
    {
        public int Id { get; set; }
        [Required,StringLength(150)]
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
