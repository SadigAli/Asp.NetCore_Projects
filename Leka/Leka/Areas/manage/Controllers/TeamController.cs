using Leka.DAL;
using Leka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Areas.manage.Controllers
{
    [Area("manage")]
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;

        public TeamController(AppDbContext context)
        {
            _context = context;
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
        public IActionResult Create(Team team)
        {
            if (!ModelState.IsValid) return View(team);
            team.SosialIcons = team.Sosials.Select(s => new SosialIcon { Name = s }).ToList();
            foreach (string link in team.Links)
            {
                team.SosialIcons.ForEach(x => 
                { x.Link = link;
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

            _context.Teams.Add(team);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            Team team = _context.Teams.Include(x=>x.SosialIcons).FirstOrDefault(x => x.Id == id);
            if (team == null) return NotFound();
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Team team)
        {
            if (id == null) return NotFound();
            Team teamDb = _context.Teams.Include(x => x.SosialIcons).FirstOrDefault(x => x.Id == id);
            if (teamDb == null) return NotFound();

            if (!ModelState.IsValid) return View(team);

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
