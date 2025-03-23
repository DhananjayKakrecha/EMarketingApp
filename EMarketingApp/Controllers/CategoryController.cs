// Controllers/CategoryController.cs
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMarketingApp.Models;


namespace EMarketingApp.Controllers
{
    // Ensure that only logged-in admins (or authorized users) can access these actions.
    public class CategoryController : Controller
    {
        private EMarketingContext db = new EMarketingContext();

        // GET: Category/Index
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload.
                if (file != null && file.ContentLength > 0)
                {
                    // Generate a unique file name to avoid overwriting.
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Define the path to store the image. For example, in Content/CategoryImages.
                    string path = Path.Combine(Server.MapPath("~/Content/CategoryImages"), fileName);

                    // Save the file to disk.
                    file.SaveAs(path);

                    // Store the relative path or file name in the database.
                    model.Cat_Image = fileName;
                }
                else
                {
                    // Optionally, add an error if the image is required.
                    ModelState.AddModelError("file", "Please select an image.");
                    return View(model);
                }

                // Save new category to database.
                db.Categories.Add(model);
                db.SaveChanges();

                // Redirect to Index page showing the updated list.
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            //if (id == null)
            //    return HttpNotFound();

            //var category = db.Categories.Find(id);
            //if (category == null)
            //    return HttpNotFound();

            //return View(category);

            var category = db.Categories.Find(id);
            if (category != null)
            {
                // Optionally delete the image file from disk.
                if (!string.IsNullOrEmpty(category.Cat_Image))
                {
                    string fullPath = Server.MapPath("~/Content/CategoryImages/" + category.Cat_Image);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                db.Categories.Remove(category);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);
            if (category != null)
            {
                // Optionally delete the image file from disk.
                if (!string.IsNullOrEmpty(category.Cat_Image))
                {
                    string fullPath = Server.MapPath("~/Content/CategoryImages/" + category.Cat_Image);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                db.Categories.Remove(category);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}