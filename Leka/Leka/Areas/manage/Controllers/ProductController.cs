using Leka.DAL;
using Leka.Extensions;
using Leka.Helpers;
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
            if (product.ProductTagIds != null)
            {
                foreach (int tagId in product.ProductTagIds)
                {
                    product.ProductTags.Add(new ProductTag
                    {
                        TagId = tagId
                    });
                }

            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            Product product = _context.Products
                                .Include(x=>x.ProductTags).ThenInclude(x=>x.Tag)
                                .Include(x=>x.ProductColors).ThenInclude(x=>x.Color)
                                .Include(x=>x.Category)
                                .FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            product.ProductTagIds = product.ProductTags.Select(x => x.TagId).ToList();
            product.ProductColorIds = product.ProductColors.Select(x => x.ColorId).ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Product product)
        {
            if (id == null) return NotFound();
            Product productDb = _context.Products
                                .Include(x => x.ProductTags).ThenInclude(x => x.Tag)
                                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                                .Include(x => x.Category)
                                .FirstOrDefault(x => x.Id == id);
            if (productDb == null) return NotFound();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid) return View(productDb);

            productDb.CategoryId = product.CategoryId;
            productDb.CostPrice = product.CostPrice;
            productDb.Name = product.Name;
            productDb.Description = product.Description;
            productDb.DiscountPrice = product.DiscountPrice;
            productDb.SalePrice = product.SalePrice;

            productDb.ProductColors.RemoveAll(x => !product.ProductColorIds.Contains(x.ColorId));
            if (product.ProductColorIds != null)
            {
                foreach (int colorId in product.ProductColorIds.Where(x=>!productDb.ProductColors.Any(pc=>pc.ColorId==x)))
                {
                    productDb.ProductColors.Add(new ProductColor
                    {
                        ColorId=colorId,
                        ProductId=(int)id
                    });
                }
            }

            productDb.ProductTags.RemoveAll(x => !product.ProductTagIds.Contains(x.TagId));
            if (product.ProductTagIds != null)
            {
                foreach (int tagId in product.ProductTagIds.Where(x=>!productDb.ProductTags.Any(pt=>pt.TagId==x)))
                {
                    productDb.ProductTags.Add(new ProductTag
                    {
                        ProductId = (int)id,
                        TagId = tagId
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            Product productDb = _context.Products
                                .Include(x => x.ProductTags).ThenInclude(x => x.Tag)
                                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                                .Include(x => x.Category)
                                .FirstOrDefault(x => x.Id == id);
            if (productDb == null) return NotFound();

            try
            {
                if (productDb.ProductColors != null)
                {
                    foreach (ProductColor productColor in productDb.ProductColors)
                    {
                        if (productColor.ProductImages != null)
                        {
                            foreach (ProductImage productImage in productColor.ProductImages)
                            {
                                _context.ProductImages.Remove(productImage);
                            }
                            _context.ProductColors.Remove(productColor);
                        }
                    }
                }

                if (productDb.ProductTags != null)
                {
                    foreach (ProductTag productTag in productDb.ProductTags)
                    {
                        _context.ProductTags.Remove(productTag);
                    }
                }

                _context.Products.Remove(productDb);
                _context.SaveChanges();

                return Json(new
                {
                    Code = 204,
                    Message = "Item has been deleted successfully!"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    Code = 500,
                    Message = "Something went wrong!"
                });
            }
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
            ProductImage poster = productColorDb.ProductImages.FirstOrDefault(x => x.PosterStatus == true);
            ProductImage hover = productColorDb.ProductImages.FirstOrDefault(x => x.PosterStatus == false);
            #region PosterFileAdded
            if (poster == null)
            {
                if (productColor.PosterImage == null)
                {
                    ModelState.AddModelError("PosterImage", "Please, select file");
                    return View(productColorDb);
                }
                if(!productColor.PosterImage.CheckContent()) ModelState.AddModelError("PosterImage", "Please, select a file in the correct format");
                if(productColor.PosterImage.CheckSize(1024)) ModelState.AddModelError("PosterImage", "File's length cann't be greater than specified size");
                if (!ModelState.IsValid) return View(productColorDb);

                productColorDb.ProductImages.Add(new ProductImage
                {
                    PosterStatus = true,
                    Image = await productColor.PosterImage.SaveImage(path)
                }) ;
            }
            else
            {
                if (productColor.PosterImage != null)
                {
                    Helper.FileDelete(path, poster.Image);
                    poster.Image = await productColor.PosterImage.SaveImage(path);
                }
            }
            #endregion

            #region HoverFileAdded
            if (hover == null)
            {
                if (productColor.HoverImage == null)
                {
                    ModelState.AddModelError("HoverImage", "Please, select file");
                    return View(productColorDb);
                }
                if (!productColor.HoverImage.CheckContent()) ModelState.AddModelError("HoverImage", "Please, select a file in the correct format");
                if (productColor.HoverImage.CheckSize(1024)) ModelState.AddModelError("HoverImage", "File's length cann't be greater than specified size");
                if (!ModelState.IsValid) return View(productColorDb);

                productColorDb.ProductImages.Add(new ProductImage
                {
                    PosterStatus = false,
                    Image = await productColor.HoverImage.SaveImage(path)
                });
            }
            else
            {
                if (productColor.HoverImage != null)
                {
                    Helper.FileDelete(path, hover.Image);
                    hover.Image = await productColor.HoverImage.SaveImage(path);
                }
            }
            #endregion

            #region SimpleFilesAdded

            List<ProductImage> productImages = productColorDb.ProductImages.Where(x => x.PosterStatus==null && !productColor.ImageIds.Contains(x.Id)).ToList();
            foreach (ProductImage image in productImages)
            {
                Helper.FileDelete(path, image.Image);
                productColorDb.ProductImages.Remove(image);
            }
            if (productColor.Images != null)
            {
                foreach (IFormFile file in productColor.Images)
                {
                    if (!file.CheckContent()) ModelState.AddModelError("Images", "Please, select a file in the correct format");
                    if (file.CheckSize(1024)) ModelState.AddModelError("Images", "File's length cann't be greater than specified size");
                    if (!ModelState.IsValid) return View(productColorDb);

                    productColorDb.ProductImages.Add(new ProductImage
                    {
                        PosterStatus=null,
                        Image = await file.SaveImage(path)
                    });
                }
            }
            #endregion

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteColor(int? productId, int? colorId)
        {
            if (productId == null || colorId == null) return NotFound();
            ProductColor productColor = _context.ProductColors
                                        .Include(x => x.ProductImages)
                                        .Include(x => x.Product)
                                        .Include(x => x.Color)
                                        .FirstOrDefault(x => x.ColorId == colorId && x.ProductId == productId);
            if (productColor == null) return NotFound();

            try
            {
                if (productColor.ProductImages != null)
                {
                    foreach (ProductImage productImage in productColor.ProductImages)
                    {
                        _context.ProductImages.Remove(productImage);
                    }
                }
                _context.ProductColors.Remove(productColor);
                _context.SaveChanges();
                return Json(new
                {
                    Code=204,
                    Message= "Item has been deleted successfully!"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    Code = 500,
                    Message = "Something went wrong!"
                });
            }
        }        
    }
}
