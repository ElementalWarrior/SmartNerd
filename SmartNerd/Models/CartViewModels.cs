using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartNerd.Models.CartViewModels
{
    public class Cart
    {
        public Models.Account Account { get; set; }
        public List<Models.Menu.Product> Products { get; set; }

        public Decimal Total { get; set; }
    }
}