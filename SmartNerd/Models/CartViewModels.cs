using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartNerd.Models.CartViewModels
{
    public class CheckoutPage
    {
        public List<Models.Menu.Product> Products { get; set; }

        public Decimal Total { get; set; }
    }
    public class AddressPage
    {
        public int AddressToUse { get; set; }
        public Boolean SaveAddress { get; set; }
        public Address CartAddress { get; set; }
        public List<Address> BillingAddresses { get; set; }
        public List<Address> MailingAddresses { get; set; }
    }
}