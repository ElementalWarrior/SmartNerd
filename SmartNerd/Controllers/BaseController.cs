using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

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
                        String accountID = User.Identity.GetUserId();
                        if (accountID != null && Guid.Parse(accountID) != Guid.Empty)
                        {
                            _cart.AccountID = Guid.Parse(accountID);
                        }
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