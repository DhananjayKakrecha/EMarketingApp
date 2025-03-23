using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMarketingApp.Models;

namespace EMarketingApp.Controllers
{
    public class AdminController : Controller
    {
        private EMarketingContext db= new EMarketingContext();
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        //Post : Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Admin admin)
        {
            if (ModelState.IsValid)
            {
                var user = db.Admins.FirstOrDefault(a => a.Username == admin.Username && a.Password == admin.Password);
                if (user != null)
                {
                    Session["AdminId"] = user.AdminId;
                    return RedirectToAction("Index", "Category");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            return View(admin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);   
        }
    }
}