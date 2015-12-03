using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartNerd.Models;
using Microsoft.AspNet.Identity.EntityFramework;


namespace SmartNerd.Controllers {
    public class CartController: BaseController {
        //
        // GET: /Cart/
        public ActionResult Index() {
            return RedirectToAction("Checkout","Cart");
        }

        public ActionResult Checkout(bool? noProducts = null) {
            SmartNerdDataContext _context = new SmartNerdDataContext();
            Models.CartViewModels.CheckoutPage c = new Models.CartViewModels.CheckoutPage {
                Products = (from op in Cart.Products
                            join p in _context.Products on op.ProductID equals p.ProductID
                            select new Models.Menu.Product {
                                ProductID = p.ProductID,
                                ProductName = p.Name,
                                Quantity = op.Quantity,
                                Description = p.Description,
                                Price = p.Price
                            }).ToList(),
                Total = Cart.Total
            };
            if(noProducts.HasValue) {
                ModelState.AddModelError("","You must have products in your cart to continue");
            }
            return View(c);
        }
        [HttpPost]
        public ActionResult Checkout(Models.CartViewModels.CheckoutPage model) {
            SmartNerdDataContext _context = new SmartNerdDataContext();

            List<DataModels.Product> dataProducts = (from p in _context.Products
                                                     where Cart.Products.Select(pr => pr.ProductID).Contains(p.ProductID)
                                                     select p).ToList();
            foreach(var p in model.Products) {
                DataModels.Product dataProduct = dataProducts.First(pr => pr.ProductID == p.ProductID);
                Cart.Products.Where(prod => prod.ProductID == p.ProductID).ToList().ForEach(prod => prod.Quantity = p.Quantity);
                foreach(var pr in Cart.Products.Where(prod => prod.Quantity < 1)) {
                    Cart.RemoveProduct(pr.ProductID);
                }

                if(dataProduct.Inventory - p.Quantity < 0) {
                    ModelState.AddModelError("","There isn't sufficient inventory to place this order (" + dataProduct.Name + ": " + dataProduct.Inventory + ").");

                    Models.CartViewModels.CheckoutPage c = new Models.CartViewModels.CheckoutPage {
                        Products = (from op in Cart.Products
                                    join pro in _context.Products on op.ProductID equals pro.ProductID
                                    select new Models.Menu.Product {
                                        ProductID = pro.ProductID,
                                        ProductName = pro.Name,
                                        Quantity = op.Quantity,
                                        Description = pro.Description,
                                        Price = pro.Price
                                    }).ToList(),
                        Total = Cart.Total
                    };
                    return View(c);
                }
            }
            Cart.Save();
            return RedirectToAction("Address","Cart");
        }
        private Models.CartViewModels.AddressPage BuildAddressPage() {

            SmartNerdDataContext _context = new SmartNerdDataContext();

            Models.CartViewModels.AddressPage add = new Models.CartViewModels.AddressPage {
                BillingAddresses = new List<Models.Address>(),
                MailingAddresses = new List<Models.Address>(),
            };
            if(User.Identity.GetUserId() != null) {
                add = new Models.CartViewModels.AddressPage();
                List<Models.AccountAddress> addresses = (from aa in _context.AccountAddresses
                                                         join a in _context.Addresses on aa.AddressID equals a.AddressID
                                                         where aa.UserID == User.Identity.GetUserId()
                                                         select new AccountAddress {
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
            Models.Address addressInList = add.MailingAddresses.FirstOrDefault(a => a.AddressID == Cart.AddressID);
            if(Cart.AddressID != null && addressInList == null) {
                add.CartAddress = (from a in _context.Addresses
                                   where a.AddressID == Cart.AddressID
                                   select new Models.Address {
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
        public ActionResult Address() {
            if(Cart.Products.Count == 0) {
                return RedirectToAction("Checkout","Cart",new { noProducts = true });
            }
            Models.CartViewModels.AddressPage add = BuildAddressPage();
            return View(add);
        }
        [HttpPost]
        public ActionResult Address(Models.CartViewModels.AddressPage model) {
            if(model.AddressToUse != -1) {
                Cart.AddressID = model.AddressToUse;
                Cart.Save();
                if(Session["CartID"] == null) {
                    Session["CartID"] = Cart.CartID;
                }
                return RedirectToAction("Pay","Cart");
            } else {
                TryUpdateModel(model.CartAddress);
                if(ModelState.IsValid) {
                    Cart.UseNewAddress(new Address {
                        City = model.CartAddress.City,
                        Line1 = model.CartAddress.Line1,
                        Line2 = model.CartAddress.Line2,
                        StateOrProvince = model.CartAddress.StateOrProvince,
                        ZipCode = model.CartAddress.ZipCode,
                        County = model.CartAddress.County,
                        FullName = model.CartAddress.FullName
                    });
                    if(model.SaveAddress) {
                        DataModels.AccountAddress aa = new DataModels.AccountAddress {
                            UserID = User.Identity.GetUserId(),
                            AddressID = Cart.AddressID.Value,
                            AddressType = "Mailing"
                        };
                        SmartNerdDataContext _context = new SmartNerdDataContext();
                        _context.AccountAddresses.InsertOnSubmit(aa);
                        _context.SubmitChanges();
                    }
                    Cart.Save();
                    if(Session["CartID"] == null) {
                        Session["CartID"] = Cart.CartID;
                    }
                    return RedirectToAction("Pay","Cart");
                }
            }

            Models.CartViewModels.AddressPage add = BuildAddressPage();
            add.CartAddress = model.CartAddress;
            return View(add);
        }
        public ActionResult Pay() {
            if(Cart.Products == null || Cart.Products.Count == 0) {
                return RedirectToAction("Index","Home");
            }
            if(Cart.OrderID == 0) {
                return RedirectToAction("Index","Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Pay(Models.CartViewModels.PayPage model) {
            if(ModelState.IsValid) {
                if(Cart.Products == null || Cart.Products.Count == 0) {
                    return RedirectToAction("Index","Home");
                }
                if(Cart.OrderID == 0) {
                    return RedirectToAction("Index","Home");
                }
                SmartNerdDataContext _context = new SmartNerdDataContext();

                List<DataModels.Product> dataProducts = (from p in _context.Products
                                                         where Cart.Products.Select(pr => pr.ProductID).Contains(p.ProductID)
                                                         select p).ToList();
                foreach(var p in Cart.Products) {
                    DataModels.Product dataProduct = dataProducts.First(pr => pr.ProductID == p.ProductID);

                    if(dataProduct.Inventory - p.Quantity < 0) {
                        ModelState.AddModelError("","There isn't sufficient inventory to place this order (" + dataProduct.Name + ": " + dataProduct.Inventory + ").");

                        return View();
                    }
                }
                Cart.SubmitOrder(new Payment {
                    CardNumber = model.CardNumber,
                    CardType = model.CardType,
                    PayPalID = model.PayPalUsername
                });
                Session["CartID"] = null;

                return RedirectToAction("ThankYou","Cart",new { OrderId = Cart.OrderID });
            }
            return View();
        }

        public ActionResult ThankYou(int OrderId) {
            SmartNerdDataContext db = new SmartNerdDataContext();

            var order = db.Orders.Single(a => a.OrderID == OrderId);
            var orderModel = new Models.Order.Order {
                Address = new Models.Address {
                    AddressID = order.Address.AddressID,
                    City = order.Address.City,
                    County = order.Address.County,
                    FullName = order.Address.FullName,
                    Line1 = order.Address.FullName,
                    Line2 = order.Address.Line2,
                    StateOrProvince = order.Address.StateOrProvince,
                    ZipCode = order.Address.ZipCode
                },
                DatePlaced = order.DatePlaced.Value,
                OrderID = Cart.OrderID,
                Total = order.OrderTotal,
                Products = (from a in order.OrderProducts
                            join b in db.Products on a.ProductID equals b.ProductID
                            select new Models.Order.Product {
                                Name = b.Name,
                                Price = b.Price,
                                ProductID = b.ProductID,
                                Quantity = a.Quantity
                            }).ToList()
            };

            return View(orderModel);
        }
    }
}