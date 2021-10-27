using Leka.DAL;
using Leka.Extensions;
using Leka.Helpers;
using Leka.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Areas.manage.Controllers
{
    [Area("manage")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        public SettingController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            Setting setting = _context.Settings.FirstOrDefault();
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Setting setting)
        {
            if (!ModelState.IsValid) return View("Index",setting);
            Setting settingDb = _context.Settings.FirstOrDefault();
            string path = Path.Combine(_env.WebRootPath, "uploads", "setting");

            // Added new setting datas
            if (settingDb == null)
            {
                Setting newSetting = new Setting
                {
                    AboutTitle = setting.AboutTitle,
                    AboutUs = setting.AboutUs,
                    Address = setting.Address,
                    ContactInfo = setting.ContactInfo,
                    Copyright = setting.Copyright,
                    Email = setting.Email,
                    FacebookLink = setting.FacebookLink,
                    FooterAbout = setting.FooterAbout,
                    GoogleLink = setting.GoogleLink,
                    Phone = setting.Phone,
                    PinterestLink = setting.PinterestLink,
                    SkypeLink = setting.SkypeLink,
                    TwitterLink = setting.TwitterLink,
                    WebPage = setting.WebPage,
                    WorkGraphic = setting.WorkGraphic
                };
                // Inserted Header Logo

                if (setting.HeaderImage == null)
                {
                    ModelState.AddModelError("HeaderImage", "HeaderLogo null ola bilmez");
                    return View("Index", setting);
                }
                if(setting.FooterImage == null)
                {
                    ModelState.AddModelError("FooterImage", "HeaderLogo null ola bilmez");
                    return View("Index", setting);
                }
                if (!setting.HeaderImage.CheckContent())
                {
                    ModelState.AddModelError("HeaderImage", "Fayl shekil formatinda olmalidir");
                    return View("Index",setting);
                }
                if (setting.HeaderImage.CheckSize(200))
                {
                    ModelState.AddModelError("HeaderImage", "Faylin olchusu 200kb-dan chox olmamalidi!");
                    return View("Index",setting);
                }

                // Inserted Footer Logo
                if (!setting.FooterImage.CheckContent())
                {
                    ModelState.AddModelError("FooterImage", "Fayl shekil formatinda olmalidir");
                    return View("Index", setting);
                }
                if (setting.FooterImage.CheckSize(200))
                {
                    ModelState.AddModelError("FooterImage", "Faylin olchusu 200kb-dan chox olmamalidi!");
                    return View("Index", setting);
                }

                newSetting.HeaderLogo = await setting.HeaderImage.SaveImage(path);


                newSetting.FooterLogo = await setting.FooterImage.SaveImage(path);

                // Added Other Data

                _context.Settings.Add(newSetting);
            }
            // Updated setting datas
            else
            {
                if (setting.HeaderImage != null)
                {
                    if (!setting.HeaderImage.CheckContent())
                    {
                        ModelState.AddModelError("HeaderImage", "Fayl shekil formatinda olmalidir");
                        return View("Index", setting);
                    }
                    if (setting.HeaderImage.CheckSize(200))
                    {
                        ModelState.AddModelError("HeaderImage", "Faylin olchusu 200kb-dan chox olmamalidi!");
                        return View("Index", setting);
                    }

                    Helper.FileDelete(path,settingDb.HeaderLogo);
                    settingDb.HeaderLogo = await setting.HeaderImage.SaveImage(path);
                }

                if(setting.FooterImage != null)
                {
                    if (!setting.FooterImage.CheckContent())
                    {
                        ModelState.AddModelError("FooterImage", "Fayl shekil formatinda olmalidir");
                        return View("Index", setting);
                    }
                    if (setting.FooterImage.CheckSize(200))
                    {
                        ModelState.AddModelError("FooterImage", "Faylin olchusu 200kb-dan chox olmamalidi!");
                        return View("Index", setting);
                    }

                    Helper.FileDelete(path,settingDb.FooterLogo);
                    settingDb.FooterLogo = await setting.FooterImage.SaveImage(path);

                }
                settingDb.AboutTitle = setting.AboutTitle;
                settingDb.AboutUs = setting.AboutUs;
                settingDb.Address = setting.Address;
                settingDb.ContactInfo = setting.ContactInfo;
                settingDb.Copyright = setting.Copyright;
                settingDb.Email = setting.Email;
                settingDb.FacebookLink = setting.FacebookLink;
                settingDb.FooterAbout = setting.FooterAbout;
                settingDb.GoogleLink = setting.GoogleLink;
                settingDb.Phone = setting.Phone;
                settingDb.PinterestLink = setting.PinterestLink;
                settingDb.SkypeLink = setting.SkypeLink;
                settingDb.TwitterLink = setting.TwitterLink;
                settingDb.WebPage = setting.WebPage;
                settingDb.WorkGraphic = setting.WorkGraphic;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
