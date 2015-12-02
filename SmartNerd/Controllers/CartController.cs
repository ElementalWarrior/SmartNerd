using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartNerd.Models;
using Microsoft.AspNet.Identity.EntityFramework;


namespace SmartNerd.Controllers
{
    public class CartController : BaseController
    {
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            return RedirectToAction("Checkout", "Cart");
        }

        public ActionResult Checkout()
        {
            SmartNerdDataContext _context = new SmartNerdDataContext();
            Models.CartViewModels.CheckoutPage c = new Models.CartViewModels.CheckoutPage
            {
                Products = (from op in Cart.Products
                           join p in _context.Products on op.ProductID equals p.ProductID
                           select new Models.Menu.Product
                           {
                                ProductID = p.ProductID,
                                ProductName = p.Name,
                                Quantity = op.Quantity,
                                Description = p.Description,
                                Price = p.Price
                           }).ToList(),
                Total = Cart.Total
            };
            return View(c);
        }
        private Models.CartViewModels.AddressPage BuildAddressPage()
        {

            SmartNerdDataContext _context = new SmartNerdDataContext();

            Models.CartViewModels.AddressPage add = new Models.CartViewModels.AddressPage
            {
                BillingAddresses = new List<Models.Address>(),
                MailingAddresses = new List<Models.Address>(),
            };
            if (User.Identity.GetUserId() != null)
            {
                add = new Models.CartViewModels.AddressPage();
                List<Models.AccountAddress> addresses = (from aa in _context.AccountAddresses
                                                         join a in _context.Addresses on aa.AddressID equals a.AddressID
                                                         where aa.UserID == User.Identity.GetUserId()
                                                         select new AccountAddress
                                                         {
                                                             AddressID = a.AddressID,
                                                             FullName = a.FullName,
                                                             City = a.City,
                                                             County = a.County,
                                                             Line1 = a.Line1,
                                                             Line2 = a.Line2,
                                                             StateOrProvince = a.StateOrProvince,
                                                             ZipCode = a.ZipCode,
                                                             AddressType = aa.AddressType,
                                                             AccountID = new Guid(aa.UserID)
                                                         }).ToList();
                add.BillingAddresses = addresses.Where(m => m.AddressType == "Billing").Select(m => (Models.Address)m).ToList();
                add.MailingAddresses = addresses.Where(m => m.AddressType == "Mailing").Select(m => (Models.Address)m).ToList();
            }
            if (Cart.AddressID != null)
            {
                add.CartAddress = (from a in _context.Addresses
                                   where a.AddressID == Cart.AddressID
                                   select new Models.Address
                                   {
                                       FullName = a.FullName,
                                       City = a.City,
                                       County = a.County,
                                       Line1 = a.Line1,
                                       Line2 = a.Line2,
                                       StateOrProvince = a.StateOrProvince,
                                       ZipCode = a.ZipCode
                                   }).FirstOrDefault();
            }
            return add;
        }
        public ActionResult Address()
        {
            Models.CartViewModels.AddressPage add = BuildAddressPage();
            return View(add);
        }
        [HttpPost]
        public ActionResult Address(Models.CartViewModels.AddressPage model)
        {
            if(model.AddressToUse != -1)
            {
                Cart.AddressID = model.AddressToUse;
            }
            else
            {
                Cart.UseNewAddress(new Address
                {
                    City = model.CartAddress.City,
                    Line1 = model.CartAddress.Line1,
                    Line2 = model.CartAddress.Line2,
                    StateOrProvince = model.CartAddress.StateOrProvince,
                    ZipCode = model.CartAddress.ZipCode,
                    County = model.CartAddress.County,
                    FullName = model.CartAddress.FullName
                });
            }
            Cart.Save();
            return View(BuildAddressPage());
        }
	}
}