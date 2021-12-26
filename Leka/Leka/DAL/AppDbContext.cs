using Leka.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.DAL
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<AboutSkill> AboutSkills { get; set; }
        public DbSet<SosialIcon> SosialIcons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductBasket> ProductBaskets { get; set; }
    }
}
