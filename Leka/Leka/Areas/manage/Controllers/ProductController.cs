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
            List<Product> products = _context.Products.Include(x => x.Category)
                                        .Include(x=>x.ProductColors).ThenInclude(x => x.ProductImages)
                                        .Include(x=>x.ProductColors).ThenInclude(x=>x.Color)
                                        .ToList();
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
        public IActionResult Create(Product product)
        {
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid) return View();

            product.ProductTags = new List<ProductTag>();
            product.ProductColors = new List<ProductColor>();

            foreach (int colorId in product.ProductColorIds)
            {
                product.ProductColors.Add(new ProductColor
                {
                    ColorId = colorId
                });
            }

            foreach (int tagId in product.ProductTagIds)
            {
                product.ProductTags.Add(new ProductTag
                {
                    TagId = tagId
                });
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddImage(int? productId, int? colorId)
        {
            if (productId == null || colorId == null) return NotFound();
            ProductColor productColor = _context.ProductColors
                                        .Include(x=>x.ProductImages)
                                        .Include(x=>x.Product)
                                        .Include(x=>x.Color)
                                        .FirstOrDefault(x=>x.ColorId==colorId && x.ProductId==productId);
            if (productColor == null) return NotFound();

            return View(productColor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int? productId, int? colorId,ProductColor productColor)
        {
            if (productId == null || colorId == null) return NotFound();
            ProductColor productColorDb = _context.ProductColors
                                        .Include(x => x.ProductImages)
                                        .Include(x => x.Product)
                                        .Include(x => x.Color)
                                        .FirstOrDefault(x => x.ColorId == colorId && x.ProductId == productId);
            if (productColorDb == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "uploads", "products");
            productColorDb.ProductImages = new List<ProductImage>();

            #region PosterImageAdded
            if (productColor.PosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "Poster sheklini daxil edin!");
                return View(productColorDb);
            }

            if (!productColor.PosterImage.CheckContent())
            {
                ModelState.AddModelError("PosterImage", "Duzgun formatda shekil daxil edin!");
                return View(productColorDb);
            }

            if (productColor.PosterImage.CheckSize(200))
            {
                ModelState.AddModelError("PosterImage", "Sheklin uzunlugu verilen olchuden chox olmamalidi");
                return View(productColorDb);
            }

            productColorDb.ProductImages.Add(new ProductImage
            {
                Image = await productColor.PosterImage.SaveImage(path),
                PosterStatus = true
            });
            #endregion

            #region HoverImageAdded
            if (productColor.HoverImage == null)
            {
                ModelState.AddModelError("HoverImage", "Poster sheklini daxil edin!");
                return View(productColorDb);
            }

            if (!productColor.HoverImage.CheckContent())
            {
                ModelState.AddModelError("HoverImage", "Duzgun formatda shekil daxil edin!");
                return View(productColorDb);
            }

            if (productColor.HoverImage.CheckSize(200))
            {
                ModelState.AddModelError("HoverImage", "Sheklin uzunlugu verilen olchuden chox olmamalidi");
                return View(productColorDb);
            }

            productColorDb.ProductImages.Add(new ProductImage
            {
                Image = await productColor.HoverImage.SaveImage(path),
                PosterStatus = false
            });
            #endregion

            #region SimpleImagesAdded
            foreach (IFormFile file in productColor.Images)
            {
                if (!file.CheckContent())
                {
                    ModelState.AddModelError("Images", "Duzgun formatda shekil daxil edin!");
                    return View(productColorDb);
                }

                if (file.CheckSize(200))
                {
                    ModelState.AddModelError("Images", "Sheklin uzunlugu verilen olchuden chox olmamalidi");
                    return View(productColorDb);
                }
                productColorDb.ProductImages.Add(new ProductImage
                {
                    Image = await file.SaveImage(path),
                    PosterStatus = null
                });

            }
            #endregion

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
