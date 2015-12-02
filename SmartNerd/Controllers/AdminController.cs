using SmartNerd.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Browse(int? categoryID = null)
        {
            SmartNerdDataContext dc = new SmartNerdDataContext();
            List<Product> prods;
            //get all products
            if(categoryID == null)
            {
                prods = (from p in dc.Products
                           select new Product
                           {
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
                       select new Product
                        {
                            ProductName = p.Name,
                            ProductID = p.ProductID,
                            Price = p.Price,
                            Description = p.Description
                        }).ToList();
            }
            BrowsePage mp = new BrowsePage {
                Categories = (from c in dc.Categories
                            select new Category
                            {
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
                                ProductName = a.Name }).ToList();
            }

            model.Products = products;

            return View(model);
        }
    }
}