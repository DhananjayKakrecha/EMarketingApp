﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMarketingApp.Models;
using System.Data.Entity;

namespace EMarketingApp.Controllers
{
    public class ProductController : Controller
    {
        private EMarketingContext db = new EMarketingContext();

        // GET: Product/FilterProducts
        public PartialViewResult FilterProducts(int categoryId)
        {
            if (Session["AnalystId"] == null)
            {
                return PartialView("_ProductGrid",Enumerable.Empty<Product>());
            }

            int analystId = (int)Session["AnalystId"];

            //var products = db.Products.Where(p => p.CategoryId == categoryId).ToList();
            //return PartialView("_ProductGrid", products);

            // Fetch the filtered products for the logged-in analyst and selected category
            var filteredProducts = db.Products
                .Where(p => p.AnalystId == analystId && p.CategoryId == categoryId)
                .ToList();

            return PartialView("~/Views/Product/_ProductGrid.cshtml", filteredProducts);
        }

        // GET: Product/Create
        // GET: Product/Create
        public ActionResult Create()
        {
            // Populate categories for the dropdown
            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Product model, HttpPostedFileBase file)
        //{
        //    // Handle product upload...
        //    // Save product to database...
        //    return RedirectToAction("Dashboard", "Analyst");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Product model, HttpPostedFileBase file)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Handle file upload logic if needed...
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //            string path = Path.Combine(Server.MapPath("~/Content/ProductImages"), fileName);
        //            file.SaveAs(path);
        //            model.ProductImage = fileName;
        //        }

        //        // Add product to the database
        //        db.Products.Add(model);
        //        db.SaveChanges();
        //        return RedirectToAction("Dashboard", "Analyst");
        //    }
        //    return View(model);
        //}

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (Session["AnalystId"] == null)
                {
                    return RedirectToAction("Login", "Analyst");
                }

                model.AnalystId = (int)Session["AnalystId"];
                // Handle file upload logic for the product image
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                    string path = Server.MapPath("~/Content/ProductImages/" + fileName);
                    file.SaveAs(path);
                    model.ProductImage = fileName;
                }

                // Save the product in the database
                model.Likes = 0;
                model.Dislikes = 0;
                db.Products.Add(model);
                db.SaveChanges();

                // Redirect to the dashboard after successful addition
                return RedirectToAction("Dashboard", "Analyst");
            }

            // Repopulate the dropdown if there was an error
            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "CategoryName", model.CategoryId);
            return View(model);
        }

        // GET: Product/SetDisplay/5
        public ActionResult SetDisplay(int id)
        {
            var product = db.Products.Find(id);
            if (product == null || Session["AnalystId"] == null || product.AnalystId != (int)Session["AnalystId"])
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Product/SetDisplay/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisplay(int id, DateTime displayUntil)
        {
            var product = db.Products.Find(id);
            if (product == null || Session["AnalystId"] == null || product.AnalystId != (int)Session["AnalystId"])
            {
                return HttpNotFound();
            }

            product.DisplayUntil = displayUntil;
            db.SaveChanges();

            return RedirectToAction("Dashboard", "Analyst");
        }

        public ActionResult Ranking()
        {
            // Query and filter products by the highest number of likes
            var rankedProducts = db.Products
                .Where(p => p.Likes != null) // Exclude products with null likes
                .OrderByDescending(p => p.Likes) // Sort by Likes in descending order
                .ToList();

            return View(rankedProducts);
        }

        public ActionResult Details(int id)
        {
            var product = db.Products
                .Include(p => p.Category) // Include category details
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Check if the product is already added to the main site
            bool isAddedToMainSite = db.MainSiteProducts.Any(m => m.ProductId == id);

            var feedbacks = db.Feedbacks
                .Include(f => f.Marketer) // Include marketer details
                .Where(f => f.ProductId == id)
                .ToList();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                Feedbacks = feedbacks,
                IsAddedToMainSite = isAddedToMainSite // Pass the status to the view
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddToMainSite(int productId)
        {
            // Check if the product is already added
            if (!db.MainSiteProducts.Any(m => m.ProductId == productId))
            {
                db.MainSiteProducts.Add(new MainSiteProduct { ProductId = productId });
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        public ActionResult RemoveFromMainSite(int productId)
        {
            // Find the MainSiteProduct entry and remove it
            var entry = db.MainSiteProducts.FirstOrDefault(m => m.ProductId == productId);
            if (entry != null)
            {
                db.MainSiteProducts.Remove(entry);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = productId });
        }

        public ActionResult MainSiteProducts()
        {
            var products = db.MainSiteProducts
                .Select(m => m.Product)
                .ToList();

            return View(products);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Populate categories for the dropdown list
            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);

            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(model.ProductId);
                if (product == null)
                {
                    return HttpNotFound();
                }

                // Update product fields
                product.ProductName = model.ProductName;
                product.Description = model.Description;
                product.Price = model.Price;
                product.EmailAddress = model.EmailAddress;
                product.CategoryId = model.CategoryId;
                product.DisplayUntil = model.DisplayUntil;

                // Handle image upload (if a new image is provided)
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                    string path = Server.MapPath("~/Content/ProductImages/" + fileName);
                    file.SaveAs(path);

                    product.ProductImage = fileName; // Update the product image
                }

                db.SaveChanges();
                return RedirectToAction("Dashboard", "Analyst");
            }

            // If validation fails, repopulate categories for dropdown
            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "CategoryName", model.CategoryId);
            return View(model);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Dashboard", "Analyst");
        }

        // POST: Product/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var product = db.Products.Find(id);
        //    if (product != null)
        //    {
        //        db.Products.Remove(product);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Dashboard", "Analyst");
        //}
    }
}