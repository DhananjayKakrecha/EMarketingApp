using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMarketingApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {
            // Clear the user session
            Session.Clear();
            Session.Abandon();

            // Redirect to the login page (you can modify the route as needed)
            return RedirectToAction("Index", "Home");
        }
    }
}