using Leka.DAL;
using Leka.Models;
using Leka.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _accessor;

        public LayoutService(AppDbContext context, IHttpContextAccessor accessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _accessor = accessor;
            _userManager = userManager;
        }

        public async Task<List<ProductBasketVM>> GetBasketItems()
        {
            List<ProductBasketVM> productBaskets = new List<ProductBasketVM>();
            AppUser user = _accessor.HttpContext.User.Identity.IsAuthenticated ? await _userManager.FindByNameAsync(_accessor.HttpContext.User.Identity.Name) : null;
            if (user == null)
            {
                if (_accessor.HttpContext.Request.Cookies["basket"] != null)
                {
                    productBaskets = JsonConvert.DeserializeObject<List<ProductBasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
                }
            }else{
                productBaskets = _context.ProductBaskets.Where(x=>x.AppUserId==user.Id).Select(x=>new ProductBasketVM{
                    CategoryName=x.CategoryName,
                    Count = x.Count,
                    Image = x.Image,
                    ProductId = x.ProductId,
                    SalePrice = x.SalePrice,
                    ProductName = x.ProductName
                }).ToList();
            }

            return productBaskets;
        }

        public Setting GetSetting()
        {
            return _context.Settings.FirstOrDefault();
        }
    }
}
