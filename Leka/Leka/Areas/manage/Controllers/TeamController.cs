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
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;

        public TeamController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Team> teams = _context.Teams.Include(x => x.SosialIcon).ToList();
            return View(teams);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            Team team = _context.Teams.Include(x=>x.SosialIcon).FirstOrDefault(x => x.Id == id);
            if (team == null) return NotFound();
            return View(team);
        }
    }
}
