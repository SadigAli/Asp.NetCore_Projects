using System.Collections.Generic;
using System.Linq;
using Leka.DAL;
using Leka.Models;
using Leka.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leka.Areas.manage.Controllers
{
    [Area("manage")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Order> orders = _context.Orders.Include(x=>x.OrderItems).OrderByDescending(x=>x.Date).ToList();
            return View(orders);
        }
        public IActionResult Detail(int? Id)
        {
            if(Id==null) return NotFound();
            Order order = _context.Orders.Include(x=>x.OrderItems).FirstOrDefault(x=>x.Id==Id);
            if(order==null) return NotFound();
            return View(order);
        }

        public IActionResult Accept(int? Id)
        {
            TempData["IsSuccess"]=true;
            if(Id==null) return NotFound();
            Order order = _context.Orders.Include(x=>x.OrderItems).FirstOrDefault(x=>x.Id==Id);
            if(order==null) return NotFound();
            order.OrderStatus = OrderStatus.Accepted;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Reject(int? Id)
        {
            TempData["IsError"]=true;
            if(Id==null) return NotFound();
            Order order = _context.Orders.Include(x=>x.OrderItems).FirstOrDefault(x=>x.Id==Id);
            if(order==null) return NotFound();
            order.OrderStatus = OrderStatus.Rejected;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}