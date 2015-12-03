using SmartNerd.Models.Admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Controllers {
    public class AdminController: Controller {
        // GET: Admin
        [Authorize(Roles = "Administrator")]
        public ActionResult Index() {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Browse(int? categoryID = null) {
            SmartNerdDataContext dc = new SmartNerdDataContext();
            List<Product> prods;
            //get all products
            if(categoryID == null) {
                prods = (from p in dc.Products
                         select new Product {
                             ProductName = p.Name,
                             ProductID = p.ProductID,
                             Price = p.Price,
                             Description = p.Description
                         }).ToList();
            } else {
                prods = (from c in dc.Categories
                         join ce in dc.CategoryEntries on c.CategoryID equals ce.CategoryID
                         join p in dc.Products on ce.ProductID equals p.ProductID
                         where c.CategoryID == categoryID.Value
                         select new Product {
                             ProductName = p.Name,
                             ProductID = p.ProductID,
                             Price = p.Price,
                             Description = p.Description
                         }).ToList();
            }
            BrowsePage mp = new BrowsePage {
                Categories = (from c in dc.Categories
                              select new Category {
                                  CategoryID = c.CategoryID,
                                  CategoryName = c.Name
                              }).ToList(),
                Products = prods
            };
            return View(mp);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Search(SearchPage model) {
            SmartNerdDataContext dc = new SmartNerdDataContext();

            List<Product> products = null;

            if(model.SearchTerm != null) {
                products = (from a in dc.Products
                            where a.Name.Contains(model.SearchTerm)
                            select new Product {
                                Description = a.Description,
                                Price = a.Price,
                                ProductID = a.ProductID,
                                ProductName = a.Name
                            }).ToList();
            }

            model.Products = products;

            return View(model);
        }

        public ActionResult Product(int productID,int? categoryID = null) {
            if(TempData["ModelState"] != null && !ModelState.Equals(TempData["ModelState"]))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);


            SmartNerdDataContext _context = new SmartNerdDataContext();

            Product prod = (from p in _context.Products
                            where p.ProductID == productID
                            select new Product {
                                ProductID = p.ProductID,
                                Price = p.Price,
                                ProductName = p.Name,
                                Description = p.Description,
                                Inventory = 0
                            }).First();

            return View(prod);
        }

        [HttpPost]
        public ActionResult Product(Product model) {
            SmartNerdDataContext db = new SmartNerdDataContext();

            var prod = db.Products.Single(a => a.ProductID == model.ProductID);
            prod.Description = model.Description;
            prod.Name = model.ProductName;
            prod.Price = model.Price;

            db.SubmitChanges();

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteProduct(Product model) {
            SmartNerdDataContext db = new SmartNerdDataContext();

            var entries = (from a in db.CategoryEntries
                           where a.ProductID == model.ProductID
                           select a).ToList();

            foreach(var entry in entries) db.CategoryEntries.DeleteOnSubmit(entry);

            var prod = db.Products.Single(a => a.ProductID == model.ProductID);
            db.Products.DeleteOnSubmit(prod);

            db.SubmitChanges();

            return RedirectToAction("Browse");
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file,Product model) {
            if(file != null && file.ContentLength > 0 && file.ContentType == "image/png") {
                var path = Path.Combine(Server.MapPath("~/Images/p"),model.ProductID.ToString()+".png");
                file.SaveAs(path);
            } else {
                ModelState.AddModelError("","Image must be a valid PNG");
                TempData["ModelState"] = ModelState;
            }


            return RedirectToAction("Product",model);
        }
    }
}