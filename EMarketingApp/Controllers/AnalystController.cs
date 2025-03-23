using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMarketingApp.Models;

namespace EMarketingApp.Controllers
{
    public class AnalystController : Controller
    {
        private EMarketingContext db = new EMarketingContext();

        // GET: Analyst/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Analyst/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Analyst model)
        {
            if (ModelState.IsValid)
            {
                var analyst = db.Analysts.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);

                if (analyst != null)
                {
                    Session["AnalystId"] = analyst.AnalystId;
                    return RedirectToAction("Dashboard", "Analyst");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid credentials.");
                }
            }
            return View(model);
        }

        // GET: Analyst/Signup
        public ActionResult Signup()
        {
            return View();
        }

        // POST: Analyst/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(Analyst model)
        {
            if (ModelState.IsValid)
            {
                db.Analysts.Add(model);
                db.SaveChanges();
                return RedirectToAction("Login", "Analyst");
            }
            return View(model);
        }

        // GET: Analyst/Dashboard
        //public ActionResult Dashboard()
        //{
        //    if (Session["AnalystId"] == null)
        //        return RedirectToAction("Login");

        //    var categories = db.Categories.ToList();
        //    return View(categories);
        //}

        //GET: Analyst/Dashboard
        public ActionResult Dashboard()
        {
            // Ensure the user is logged in
            if (Session["AnalystId"] == null)
            {
                return RedirectToAction("Login", "Analyst");
            }

            // Get the logged-in AnalystId from the session
            int analystId = (int)Session["AnalystId"];

            // Create a ViewModel containing Categories and Products filtered by AnalystId
            var viewModel = new DashboardViewModel
            {
                Categories = db.Categories.ToList(),
                Products = db.Products.Where(p => p.AnalystId == analystId).ToList() // Filter products by AnalystId
            };

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}