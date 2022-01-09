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
using Leka.Services;
using Leka.DAL;
using Leka.Models.Enums;
using System.Linq.Expressions;

namespace Leka.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _usermanager;
        private readonly LayoutService _service;
        public Dictionary<int, string> Dictionary;


        public ProductController(AppDbContext context,
                                 UserManager<AppUser> usermanager,
                                 LayoutService service)
        {
            _context = context;
            _usermanager = usermanager;
            _service = service;
        }
        public IActionResult Index(Dictionary<string, string> filters, int min = 0, int max = 350, int page = 1)
        {
            var query = _context.Products
                                .Include(x => x.Category)
                                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                                .Include(x => x.ProductColors).ThenInclude(x => x.ProductImages)
                                .Include(x => x.ProductTags).ThenInclude(x => x.Tag).AsEnumerable();

            ViewBag.Page = 1;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Filters = filters;
            if (filters.Count() > 0)
            {
                foreach (string key in filters.Keys)
                {
                    switch (key)
                    {
                        case "min":
                            query = query.Where(x => x.SalePrice - x.DiscountPrice >= double.Parse(filters[key]));
                            continue;
                        case "max":
                            query = query.Where(x => x.SalePrice - x.DiscountPrice <= double.Parse(filters[key]));
                            continue;
                        case "page":
                            if (Int32.Parse(filters[key]) != 0)
                            {
                                ViewBag.Page = Int32.Parse(filters[key]);
                            }
                            continue;
                        default:
                            int filterId = Int32.Parse(key.Split('-')[1]);
                            int identicator = Int32.Parse(key.Split('-')[0]);
                            switch (filterId)
                            {
                                case 1:
                                    query = query.Where(x => filters.ContainsKey($"{x.CategoryId}-{filterId}"));
                                    continue;
                                case 2:
                                    query = query.Where(x => x.ProductColors.Any(x => filters.ContainsKey($"{x.ColorId}-2")));
                                    continue;
                                default:
                                    break;
                            }
                            continue;
                    }

                }
            }
            ViewBag.Count = query.Count();
            ViewBag.Max = filters.ContainsKey("max") ? Int32.Parse(filters["max"]) : query.Max(x => x.SalePrice - x.DiscountPrice);
            ViewBag.Min = filters.ContainsKey("min") ? Int32.Parse(filters["min"]) : query.Min(x => x.SalePrice - x.DiscountPrice);
            List<Product> products = query.OrderByDescending(x => x.Id)
                                                .Skip(((int)ViewBag.Page - 1) * 6).Take(6).ToList().ToList();
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

        public async Task<IActionResult> Checkout()
        {
            List<ProductBasketVM> products = await _service.GetBasketItems();
            OrderVM model = new OrderVM
            {
                ProductBaskets = products
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderVM order)
        {
            List<ProductBasketVM> products = await _service.GetBasketItems();
            AppUser user = User.Identity.IsAuthenticated ? await _usermanager.FindByNameAsync(User.Identity.Name) : null;
            order.ProductBaskets = products;
            if (!ModelState.IsValid) return View(order);
            Order newOrder = new Order
            {
                Address = order.Address,
                AppUserId = user != null ? user.Id : null,
                Email = order.Email,
                Fullname = order.Name + " " + order.Lastname,
                Phone = order.Phone,
                ZipCode = order.ZipCode,
                OrderStatus = OrderStatus.Pending,
                Date = DateTime.UtcNow.AddHours(4),
            };
            newOrder.OrderItems = new List<OrderItem>();
            double totalPrice = 0;
            foreach (ProductBasketVM product in products)
            {
                totalPrice += product.SalePrice * product.Count;
                newOrder.OrderItems.Add(new OrderItem
                {
                    Count = product.Count,
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductPrice = product.SalePrice,
                });
                newOrder.TotalPrice = totalPrice;
            }
            _context.Orders.Add(newOrder);
            _context.SaveChanges();
            if (user == null)
            {
                HttpContext.Response.Cookies.Delete("basket");
            }
            else
            {
                _context.ProductBaskets.RemoveRange(_context.ProductBaskets.Where(x => x.AppUserId == user.Id));
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
