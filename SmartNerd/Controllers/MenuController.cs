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
        public ActionResult Index()
        {
            SmartNerdDataContext dc = new SmartNerdDataContext();

            Models.Menu.MenuPage mp = new Models.Menu.MenuPage{
                Categories = (from c in dc.Categories
                            select new Models.Menu.Category
                            {
                                CategoryName = c.Name
                            }).ToList(),
                Products = (from p in dc.Products
                           select new Models.Menu.Product
                           {
                               ProductName = p.Name,
                               ProductID = p.ProductID,
                               Price = p.Price,
                               Description = p.Description
                           }).ToList()
            };
            return View(mp);
        }
	}
}