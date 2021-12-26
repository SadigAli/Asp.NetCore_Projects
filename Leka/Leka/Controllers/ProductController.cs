using Leka.DAL;
using Leka.Models;
using Leka.ViewModels;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _usermanager;

        public ProductController(AppDbContext context, UserManager<AppUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }
        public IActionResult Index(int? page = 1)
        {
            var query = _context.Products
                                .Include(x => x.Category)
                                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                                .Include(x => x.ProductColors).ThenInclude(x => x.ProductImages)
                                .Include(x => x.ProductTags).ThenInclude(x => x.Tag)
                                .OrderByDescending(x => x.Id).Skip(((int)page - 1) * 6).Take(6).AsQueryable();
            ViewBag.Count = _context.Products.Count();
            ViewBag.Page = page;
            List<Product> products = query.ToList();
            return View(products);
        }

        public IActionResult Detail()
        {
            return View();
        }

        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null) return NotFound();
            AppUser user = User.Identity.IsAuthenticated ? await _usermanager.FindByNameAsync(User.Identity.Name) : null;
            Product product = _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductColors).ThenInclude(x => x.ProductImages)
                .FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();

            List<ProductBasketVM> basketProducts = new List<ProductBasketVM>();
            if (user == null)
            {
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
            }
            else
            {
                ProductBasket productBasket = _context.ProductBaskets.Where(x => x.AppUserId == user.Id).FirstOrDefault(x => x.ProductId == id);
                if (productBasket == null)
                {
                    _context.ProductBaskets.Add(new ProductBasket
                    {
                        ProductId = (int)id,
                        ProductName = product.Name,
                        CategoryName = product.Category.Name,
                        Count = 1,
                        Image = product.ProductColors.FirstOrDefault().ProductImages.FirstOrDefault(x => x.PosterStatus == true).Image,
                        SalePrice = product.SalePrice - product.DiscountPrice,
                        AppUserId = user.Id
                    });
                }
                else
                {
                    productBasket.Count++;
                }
                _context.SaveChanges();
                basketProducts = _context.ProductBaskets.Where(x => x.AppUserId == user.Id).Select(x => new ProductBasketVM
                {
                    CategoryName = x.CategoryName,
                    Count = x.Count,
                    Image = x.Image,
                    ProductId = x.ProductId,
                    SalePrice = x.SalePrice,
                    ProductName = x.ProductName
                }).ToList();
            }
            return PartialView("_BasketPartial", basketProducts);
        }

        public async Task<IActionResult> RemoveBasket(int? id)
        {
            if (id == null) return NotFound();
            List<ProductBasketVM> productBaskets = new List<ProductBasketVM>();

            AppUser user = User.Identity.IsAuthenticated ? await _usermanager.FindByNameAsync(User.Identity.Name) : null;
            if (user == null)
            {
                string basketStr = HttpContext.Request.Cookies["basket"];
                productBaskets = JsonConvert.DeserializeObject<List<ProductBasketVM>>(basketStr);
                ProductBasketVM productBasket = productBaskets.FirstOrDefault(x => x.ProductId == id);
                if (productBasket == null) return NotFound();

                productBaskets.Remove(productBasket);

                HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(productBaskets));
            }
            else
            {
                ProductBasket product = _context.ProductBaskets.FirstOrDefault(x => x.ProductId == id);
                _context.ProductBaskets.Remove(product);
                _context.SaveChanges();
                productBaskets = _context.ProductBaskets.Where(x => x.AppUserId == user.Id).Select(x => new ProductBasketVM
                {
                    CategoryName = x.CategoryName,
                    Count = x.Count,
                    Image = x.Image,
                    ProductId = x.ProductId,
                    SalePrice = x.SalePrice,
                    ProductName = x.ProductName
                }).ToList();
            }

            return PartialView("_BasketPartial", productBaskets);
        }
    }
}
