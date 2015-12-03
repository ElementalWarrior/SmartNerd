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
            SmartNerdDataContext _context = new SmartNerdDataContext();
            var rprt = (from p in _context.Payments
                        join o in _context.Orders on p.OrderID equals o.OrderID
                        where o.DatePlaced != null
                        group new { p, o } by o.DatePlaced.Value.Year + "-" +o.DatePlaced.Value.Month + "-" + o.DatePlaced.Value.Day into agg
                        select new Models.Admin.ReportEntry
                        {
                            DatePlaced = DateTime.Parse(agg.Key),
                            NumberOfOrders = agg.Count(),
                            DailyTotal = agg.Sum(a => a.o.OrderTotal)
                        }).ToList();
            return View(new Models.Admin.ReportPage
                {
                    DailyReport = rprt
                });
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
                                Inventory = p.Inventory
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
            prod.Inventory = model.Inventory;

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
                var path = Path.Combine(Server.MapPath("~/Images/p"),model.ProductID.ToString() + ".png");
                file.SaveAs(path);
            } else {
                ModelState.AddModelError("","Product Image must be a valid PNG");
                TempData["ModelState"] = ModelState;
            }


            return RedirectToAction("Product",model);
        }

        public ActionResult CreateProduct() {
            if(TempData["ModelState"] != null && !ModelState.Equals(TempData["ModelState"]))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            return View();
        }

        [HttpPost]
        public ActionResult CreateProduct(HttpPostedFileBase file,Product model) {
            SmartNerdDataContext db = new SmartNerdDataContext();

            if(file == null || file.ContentLength == 0 || file.ContentType != "image/png") {
                ModelState.AddModelError("","Product Image must be a valid PNG");
                TempData["ModelState"] = ModelState;
            }

            if(ModelState.IsValid) {
                var prod = new DataModels.Product {
                    Description = model.Description,
                    Inventory = model.Inventory,
                    Name = model.ProductName,
                    Price = model.Price
                };

                db.Products.InsertOnSubmit(prod);
                db.SubmitChanges();

                var path = Path.Combine(Server.MapPath("~/Images/p"),prod.ProductID.ToString() + ".png");
                file.SaveAs(path);
                return RedirectToAction("Product",new { productID = prod.ProductID });
            }


            return RedirectToAction("CreateProduct",model);
        }

        public ActionResult Recovery(Boolean? recover) {
            SmartNerdDataContext db = new SmartNerdDataContext();

            if(recover.HasValue) {
                ModelState.AddModelError("","System recovered");

                FileInfo file = new FileInfo(Server.MapPath("~/Recovery/ddl.sql"));
                string script = file.OpenText().ReadToEnd();
                try {
                    db.ExecuteCommand(script);
                } catch {

                }
                try {
                    db.ExecuteCommand(script);
                } catch {

                }

                file = new FileInfo(Server.MapPath("~/Recovery/menu.sql"));
                script = file.OpenText().ReadToEnd();
                try {
                    db.ExecuteCommand(script);
                } catch {

                }

                Directory.Delete(Server.MapPath("~/Images/p"),true);

                Directory.CreateDirectory(Server.MapPath("~/Images/p"));
                foreach(String image in Directory.GetFiles(Server.MapPath("~/Recovery/p"))) {
                    String fileName = Path.GetFileName(image);
                    String destFile = Path.Combine(Server.MapPath("~/Images/p"),fileName);
                    System.IO.File.Copy(image,destFile,true);
                }
            }
            return View(recover);
        }
    }
}