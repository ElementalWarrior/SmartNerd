using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Controllers {
    [Authorize]
    public class OrderController: BaseController {
        // GET: Order
        public ActionResult Index() {
            SmartNerdDataContext db = new SmartNerdDataContext();

            var orders = (from a in db.Orders
                          where a.AccountID == new Guid(User.Identity.GetUserId()) && a.DatePlaced.HasValue
                          select new Models.Order.Order {
                              Address = new Models.Address {
                                  AddressID = a.Address.AddressID,
                                  City = a.Address.City,
                                  County = a.Address.County,
                                  FullName = a.Address.FullName,
                                  Line1 = a.Address.Line1,
                                  Line2 = a.Address.Line2,
                                  StateOrProvince = a.Address.StateOrProvince,
                                  ZipCode = a.Address.ZipCode
                              },
                              DatePlaced = a.DatePlaced.Value,
                              Total = a.OrderTotal,
                              OrderID = a.OrderID,
                              Products = a.OrderProducts.Select(b => new Models.Order.Product {
                                  Name = b.Product.Name,
                                  ProductID = b.ProductID,
                                  Quantity = b.Quantity,
                                  Price = b.Product.Price
                              })
                          }).ToList();

            return View(orders);
        }
    }
}