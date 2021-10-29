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
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Color> colors = _context.Colors.ToList();
            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult  Create(Color color)
        {
            if (!ModelState.IsValid) return View();

            if (_context.Colors.Any(x => x.Code.ToLower() == color.Code.ToLower()))
            {
                ModelState.AddModelError("Code", "Bu reng movcuddur!");
                return View();
            }

            _context.Colors.Add(new Color
            {
                Name = color.Name,
                Code = color.Code
            });

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            Color color = _context.Colors.Find(id);

            if (color == null) return NotFound();

            return View(color);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id,Color color)
        {
            if (id == null) return NotFound();
            Color colorDb = _context.Colors.Find(id);

            if (colorDb == null) return NotFound();
            if(_context.Colors.Any(x=>x.Code.ToLower()==color.Code.ToLower() && x.Id != colorDb.Id))
            {
                ModelState.AddModelError("Code", "Bu reng movcuddur!");
                return View(colorDb);
            }

            colorDb.Name = color.Name;
            colorDb.Code = color.Code;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            Color color = _context.Colors.Find(id);
            if (color == null) return NotFound();

            try
            {
                _context.Colors.Remove(color);
                _context.SaveChanges();
                return Json(new
                {
                    Code = 204,
                    Message = "Item was deleted succesffully!"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    Code = 500,
                    Message = "Item wasn't deleted!"
                });
            }
        }
    }
}
