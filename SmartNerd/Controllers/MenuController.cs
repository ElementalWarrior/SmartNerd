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
                Products = prods,
                CategoryID = 0
            };
            if(categoryID != null)
            {
                mp.CategoryID = categoryID.Value;
            }
            return View(mp);
        }

        public ActionResult Search(Models.Menu.SearchPage model) {
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

        public ActionResult Product(int productID, int? categoryID = null)
        {
            SmartNerdDataContext _context = new SmartNerdDataContext();

            Models.Menu.Product prod = (from p in _context.Products
                                        where p.ProductID == productID
                                        select new Product
                                        {
                                            ProductID = p.ProductID,
                                            Price = p.Price,
                                            ProductName = p.Name,
                                            Description = p.Description,
                                            Quantity = 1,
                                            Inventory = p.Inventory
                                        }).First();

            return View(prod);
        }

        [HttpPost]
        public ActionResult Product(Product prod)
        {
            SmartNerdDataContext _context = new SmartNerdDataContext();

            if (ModelState.IsValid)
            {
                if (Cart.OrderID == 0)
                {
                    Cart.Save();
                    Session["CartID"] = Cart.CartID;
                }
                Cart.AddProduct(prod);
                Cart.Save();

            }
            return RedirectToAction("Checkout", "Cart");
        }

        public ActionResult RemoveProduct(int productID, string returnUrl)
        {
            Cart.RemoveProduct(productID);
            Cart.Save();
            return Redirect(returnUrl);
        }
	}
}