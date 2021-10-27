using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [Required,EmailAddress(ErrorMessage ="Duzgun formatda email daxil edin")]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        [StringLength(20)]
        public string Phone { get; set; }
        [Required]
        [StringLength(50)]
        public string WorkGraphic { get; set; }
        [Required]
        [StringLength(100)]
        public string WebPage { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(100)]
        public string FacebookLink { get; set; }
        [StringLength(100)]
        public string TwitterLink { get; set; }
        [StringLength(100)]
        public string PinterestLink { get; set; }
        [StringLength(100)]
        public string GoogleLink { get; set; }
        [StringLength(100)]
        public string SkypeLink { get; set; }
        [StringLength(100)]
        public string HeaderLogo { get; set; }
        [StringLength(100)]
        public string FooterLogo { get; set; }
        [Required]
        [StringLength(200)]
        public string FooterAbout { get; set; }
        [Required]
        [StringLength(200)]
        public string ContactInfo { get; set; }
        [Required]
        [StringLength(1000)]
        public string AboutUs { get; set; }
        [StringLength(100)]
        public string AboutTitle { get; set; }
        [Required]
        [StringLength(150)]
        public string Copyright { get; set; }
        [NotMapped]
        public IFormFile HeaderImage { get; set; }
        [NotMapped]
        public IFormFile FooterImage { get; set; }
    }
}
