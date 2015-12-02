using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return View();
        }
	}
}