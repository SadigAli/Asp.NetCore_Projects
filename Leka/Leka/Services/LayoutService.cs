using Leka.DAL;
using Leka.ViewModels;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _accessor;

        public LayoutService(AppDbContext context,IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public List<ProductBasketVM> GetBasketItems()
        {
            List<ProductBasketVM> productBaskets = new List<ProductBasketVM>();
            if (_accessor.HttpContext.Request.Cookies["basket"] != null)
            {
                productBaskets = JsonConvert.DeserializeObject<List<ProductBasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            return productBaskets;
        }
    }
}
