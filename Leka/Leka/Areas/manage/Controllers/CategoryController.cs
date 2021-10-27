using Leka.DAL;
using Leka.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Areas.manage.Controllers
{
    [Area("manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View();
            if (_context.Categories.Any(x => x.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Bu category artiq movcuddur");
                return View();
            }

            _context.Categories.Add(new Category { Name = category.Name });
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            Category category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id,Category category)
        {
            if (id == null) return NotFound();

            Category categoryDb = _context.Categories.Find(id);
            if (categoryDb == null) return NotFound();

            if (_context.Categories.Any(x => x.Name.ToLower() == category.Name.ToLower() && x.Id != id))
            {
                ModelState.AddModelError("Name", "Bu category artiq movcuddur");
                return View();
            }

            categoryDb.Name = category.Name;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            Category category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            try
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return Json(new
                {
                    code = 204,
                    message = "Item has been deleted successfully!"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    code = 500,
                    message = "Something went wrong!"
                });
            }
        }
    }
}
