using SmartNerd.Models.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Controllers
{
    public class MenuController : BaseController
    {
        //
        // GET: /Menu/
        public ActionResult Index(int? categoryID = null)
        {
            SmartNerdDataContext dc = new SmartNerdDataContext();
            List<Product> prods;
            //get all products
            if(categoryID == null)
            {
                prods = (from p in dc.Products
                           select new Models.Menu.Product
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
                       select new Models.Menu.Product
                        {
                            ProductName = p.Name,
                            ProductID = p.ProductID,
                            Price = p.Price,
                            Description = p.Description
                        }).ToList();
            }
            Models.Menu.MenuPage mp = new Models.Menu.MenuPage{
                Categories = (from c in dc.Categories
                            select new Models.Menu.Category
                            {
                                CategoryID = c.CategoryID,
                                CategoryName = c.Name
                            }).ToList(),
                Products = prods
            };
            return View(mp);
        }
	}
}