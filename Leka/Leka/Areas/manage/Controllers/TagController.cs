using Leka.DAL;
using Leka.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Controllers
{
    [Area("manage")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Tag> Tags = _context.Tags.ToList();
            return View(Tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid) return View();
            if (_context.Tags.Any(x => x.Name.ToLower() == tag.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Bu tag artiq movcuddur");
                return View();
            }

            _context.Tags.Add(new Tag { Name = tag.Name });
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            Tag tag = _context.Tags.Find(id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Tag tag)
        {
            if (id == null) return NotFound();

            Tag tagDb = _context.Tags.Find(id);
            if (tagDb == null) return NotFound();

            if (_context.Tags.Any(x => x.Name.ToLower() == tag.Name.ToLower() && x.Id != id))
            {
                ModelState.AddModelError("Name", "Bu tag artiq movcuddur");
                return View();
            }

            tagDb.Name = tag.Name;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            Tag tag = _context.Tags.Find(id);
            if (tag == null) return NotFound();
            try
            {
                _context.Tags.Remove(tag);
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
