using Leka.DAL;
using Leka.Extensions;
using Leka.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Areas.manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Product> products = _context.Products.Include(x => x.Category).Include(x => x.ProductImages).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            string path = Path.Combine(_env.WebRootPath,"uploads","products");
           if(!ModelState.IsValid) return View();

            product.ProductTags = product.ProductTagIds.Select(x => new ProductTag { TagId = x }).ToList();

            foreach (int tagId in product.ProductTagIds)
            {
                if(!_context.Tags.Any(x=>x.Id==tagId))
                    ModelState.AddModelError("ProductTagIds", "Bu tag movcud deyil!");

                ProductTag productTag = new ProductTag
                {
                    TagId = tagId
                };
                product.ProductTags.Add(productTag);
            }
            product.ProductImages = new List<ProductImage>();

            if (product.PosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "Poster image is required!");
            }
            else
            {
                if (product.PosterImage.CheckSize(200))
                    ModelState.AddModelError("PosterImage", "Faylin uzunlugu 200-den chox ola bilmez!");
                if (product.PosterImage.CheckContent())
                    ModelState.AddModelError("PosterImage", "Faylin image type-inda olmalidir!");
                ProductImage productImage = new ProductImage {
                    Image = await product.PosterImage.SaveImage(path),
                    PosterStatus = true
                 };
                product.ProductImages.Add(productImage);

            }

            if (product.HoverImage == null)
            {
                ModelState.AddModelError("PosterImage", "Poster image is required!");
            }
            else
            {
                if (product.HoverImage.CheckSize(200))
                    ModelState.AddModelError("HoverImage", "Faylin uzunlugu 200-den chox ola bilmez!");
                if (product.HoverImage.CheckContent())
                    ModelState.AddModelError("HoverImage", "Faylin image type-inda olmalidir!");

                ProductImage productImage = new ProductImage
                {
                    Image = await product.HoverImage.SaveImage(path),
                    PosterStatus = false
                };
                product.ProductImages.Add(productImage);

            }

            if (product.Images != null)
            {
                foreach (IFormFile image in product.Images)
                {
                    if (image.CheckSize(200))
                    {
                        continue;
                    }
                    if (image.CheckContent())
                    {
                        continue;
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await image.SaveImage(path),
                        PosterStatus = null
                    };

                    product.ProductImages.Add(productImage);
                }
            }





            foreach (int colorId in product.ProductColorIds)
            {
                if (!_context.Colors.Any(x => x.Id == colorId))
                    ModelState.AddModelError("ProductTagIds", "Bu tag movcud deyil!");

                ProductColor productTag = new ProductColor
                {
                    ColorId = colorId,
                    ProductImageId = product.ProductImages.FirstOrDefault(x=>x.PosterStatus==true).Id
                };
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
