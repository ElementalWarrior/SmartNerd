using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Controllers
{
    public class BaseController : Controller
    {
        private Cart _cart;
        public Cart Cart
        {
            get
            {
                if(_cart == null)
                {
                    if(Session["CartID"] == null)
                    {
                        _cart = new Cart();
                    }
                    else
                    {
                        _cart = new Cart((Guid)Session["CartID"]);
                    }
                }
                return _cart;
            }
        }
	}
}