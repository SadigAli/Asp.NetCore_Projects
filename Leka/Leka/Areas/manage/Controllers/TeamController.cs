using Leka.DAL;
using Leka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leka.Helpers;
using Leka.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Leka.Areas.manage.Controllers
{
    [Area("manage")]
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TeamController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Team> teams = _context.Teams.Include(x => x.SosialIcons).ToList();
            return View(teams);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            if (!ModelState.IsValid) return View(team);
            if (team.Photo == null)
            {
                ModelState.AddModelError("Image", "Image is required");
                return View();
            }
            if (!team.Photo.CheckContent())
            {
                ModelState.AddModelError("Photo", "Select file in correct format!");
                return View();
            }
            if (team.Photo.CheckSize(200))
            {
                ModelState.AddModelError("Photo", "File is too long");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "uploads", "team");
            team.SosialIcons = team.Sosials.Select(s => new SosialIcon { Name = s }).ToList();
            foreach (string link in team.Links)
            {
                team.SosialIcons.ForEach(x =>
                {
                    x.Link = link;
                    switch (x.Name)
                    {
                        case "Facebook":
                            x.Icon = "fa-facebook-f";
                            break;
                        case "Twitter":
                            x.Icon = "fa-twitter";
                            break;
                        case "Instagram":
                            x.Icon = "fa-instagram";
                            break;
                        default:
                            break;
                    }
                });
            }
            team.Image = await team.Photo.SaveImage(path);
            _context.Teams.Add(team);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            Team team = _context.Teams.Include(x => x.SosialIcons).FirstOrDefault(x => x.Id == id);
            if (team == null) return NotFound();
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Team team)
        {
            if (id == null) return NotFound();
            Team teamDb = _context.Teams.Include(x => x.SosialIcons).FirstOrDefault(x => x.Id == id);
            if (teamDb == null) return NotFound();

            if (!ModelState.IsValid) return View(team);

            string path = Path.Combine(_env.WebRootPath, "uploads", "team");
            if (team.Photo != null)
            {
                if (!team.Photo.CheckContent())
                {
                    ModelState.AddModelError("Image", "Select file in correct format!");
                    return View();
                }
                if (team.Photo.CheckSize(200))
                {
                    ModelState.AddModelError("Image", "File is too long");
                    return View();
                }
                teamDb.Image = await team.Photo.SaveImage(path);
            }

            teamDb.Name = team.Name;
            teamDb.Lastname = team.Lastname;
            teamDb.Profession = team.Profession;

            teamDb.SosialIcons.RemoveAll(x => !team.SosialIconIds.Contains(x.Id));

            if (team.Sosials != null)
            {
                List<SosialIcon> sosialIcons = team.Sosials.Select(s => new SosialIcon { Name = s }).ToList();
                foreach (string link in team.Links)
                {
                    sosialIcons.ForEach(x =>
                    {
                        x.Link = link;
                        switch (x.Name)
                        {
                            case "Facebook":
                                x.Icon = "fa-facebook-f";
                                break;
                            case "Twitter":
                                x.Icon = "fa-twitter";
                                break;
                            case "Instagram":
                                x.Icon = "fa-instagram";
                                break;
                            default:
                                break;
                        }
                    });
                }
                teamDb.SosialIcons.AddRange(sosialIcons);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            Team team = _context.Teams.Include(x => x.SosialIcons).FirstOrDefault(x => x.Id == id);
            if (team == null) return NotFound();

            try
            {
                _context.Teams.Remove(team);
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
    }
}
