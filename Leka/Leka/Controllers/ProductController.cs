using Leka.DAL;
using Leka.Models;
using Leka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? page=1)
        {
            var query = _context.Products
                                .Include(x => x.Category)
                                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                                .Include(x => x.ProductColors).ThenInclude(x => x.ProductImages)
                                .Include(x => x.ProductTags).ThenInclude(x => x.Tag)
                                .OrderByDescending(x => x.Id).Skip(((int)page-1)*6).Take(6).AsQueryable();
            ViewBag.Count = _context.Products.Count();
            ViewBag.Page = page;
            List<Product> products = query.ToList();
            return View(products);
        }

        public IActionResult Detail()
        {
            return View();
        }

        public IActionResult AddToCart(int? id)
        {
            if (id == null) return NotFound();
            Product product = _context.Products
                .Include(x=>x.Category)
                .Include(x=>x.ProductColors).ThenInclude(x=>x.ProductImages)
                .FirstOrDefault(x=>x.Id==id);
            if (product == null) return NotFound();

            List<ProductBasketVM> basketProducts=new List<ProductBasketVM>();
            string basketStr = HttpContext.Request.Cookies["basket"];
            if (basketStr == null)
            {
                basketProducts.Add(new ProductBasketVM
                {
                    ProductId = product.Id,
                    CategoryName = product.Category.Name,
                    Count = 1,
                    Image = product.ProductColors.FirstOrDefault()?.ProductImages.FirstOrDefault(x => x.PosterStatus == true)?.Image,
                    ProductName = product.Name,
                    SalePrice = product.DiscountPrice == 0 ? product.SalePrice : product.SalePrice - product.DiscountPrice
                });
            }
            else
            {
                basketProducts = JsonConvert.DeserializeObject<List<ProductBasketVM>>(basketStr);
                ProductBasketVM productBasket = basketProducts.FirstOrDefault(x => x.ProductId == product.Id);
                if (productBasket != null)
                {
                    productBasket.Count++;
                }
                else
                {
                    basketProducts.Add(new ProductBasketVM
                    {
                        ProductId = product.Id,
                        CategoryName = product.Category.Name,
                        Count = 1,
                        Image = product.ProductColors.FirstOrDefault()?.ProductImages.FirstOrDefault(x => x.PosterStatus == true)?.Image,
                        ProductName = product.Name,
                        SalePrice = product.DiscountPrice == 0 ? product.SalePrice : product.SalePrice - product.DiscountPrice
                    });
                }
            }
            
            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketProducts));
            return PartialView("_BasketPartial", basketProducts);
            //return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveBasket(int? id)
        {
            if (id == null) return NotFound();
            string basketStr = HttpContext.Request.Cookies["basket"];
            List<ProductBasketVM> productBaskets = JsonConvert.DeserializeObject<List<ProductBasketVM>>(basketStr);
            ProductBasketVM productBasket = productBaskets.FirstOrDefault(x=>x.ProductId==id);
            if (productBasket == null) return NotFound();

            productBaskets.Remove(productBasket);

            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(productBaskets));

            return PartialView("_BasketPartial",productBaskets);
        }
    }
}
