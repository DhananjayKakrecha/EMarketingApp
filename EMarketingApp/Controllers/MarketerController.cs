using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMarketingApp.Models;

namespace EMarketingApp.Controllers
{
    public class MarketerController : Controller
    {
        private EMarketingContext db = new EMarketingContext();

        // GET: Marketer/Login
        public ActionResult Login() => View();

        // POST: Marketer/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Marketer model)
        {
            var marketer = db.Marketers.FirstOrDefault(m => m.Username == model.Username && m.Password == model.Password);
            if (marketer != null)
            {
                Session["MarketerId"] = marketer.MarketerId;
                return RedirectToAction("Demo", "Marketer");
            }

            ModelState.AddModelError("", "Invalid credentials.");
            return View(model);
        }

        // GET: Marketer/Signup
        public ActionResult Signup() => View();

        // POST: Marketer/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(Marketer model)
        {
            if (ModelState.IsValid)
            {
                db.Marketers.Add(model);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Marketer/Demo
        public ActionResult Demo()
        {
            // Ensure marketer is logged in
            if (Session["MarketerId"] == null)
            {
                return RedirectToAction("Login", "Marketer");
            }

            // Create a ViewModel instance
            var viewModel = new DemoPageViewModel
            {
                Categories = db.Categories.ToList(), // Fetch all categories
                Products = db.Products
                    .Where(p => p.DisplayUntil >= DateTime.Now) // Fetch products with active visibility
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: Marketer/FilterProducts
        public PartialViewResult FilterProducts(int categoryId)
        {
            // Ensure marketer is logged in
            if (Session["MarketerId"] == null)
            {
                return PartialView("_ProductGrid", Enumerable.Empty<Product>()); // Return an empty list
            }

            // Fetch products with active visibility (DisplayUntil > current time) for the selected category
            var products = db.Products
                .Where(p => p.CategoryId == categoryId && p.DisplayUntil >= DateTime.Now)
                .ToList();

            return PartialView("~/Views/Marketer/_ProductGrid.cshtml", products);
        }

        [HttpPost]
        public JsonResult Vote(int productId, string action)
        {
            int marketerId = (int)Session["MarketerId"];

            // Check if the marketer has already voted
            var existingVote = db.Votes.FirstOrDefault(v => v.ProductId == productId && v.MarketerId == marketerId);
            if (existingVote != null)
            {
                return Json(new { success = false, message = "You have already voted for this product." });
            }

            // Add a new vote
            db.Votes.Add(new Vote { ProductId = productId, MarketerId = marketerId, Action = action });

            // Update the product's like or dislike count
            var product = db.Products.Find(productId);
            if (action == "Like")
            {
                product.Likes++;
            }
            else if (action == "Dislike")
            {
                product.Dislikes++;
            }

            db.SaveChanges();

            return Json(new { success = true, message = $"{action} registered successfully!" });
        }

        [HttpPost]
        public ActionResult SubmitFeedback(int productId, string feedback)
        {
            int marketerId = (int)Session["MarketerId"];

            // Save feedback to the database
            db.Feedbacks.Add(new Feedback
            {
                ProductId = productId,
                MarketerId = marketerId,
                FeedbackText = feedback
            });

            db.SaveChanges();

            return RedirectToAction("Demo");
        }
    }
}