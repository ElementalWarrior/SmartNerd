using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class PayPage
    {
        [Required]
        [StringLength(16, MinimumLength=16)]
        public String CardNumber { get; set; }
        [Required]
        [StringLength(20)]
        public String CardType { get; set; }
        [Required]
        [StringLength(50)]
        public String PayPalUsername { get; set; }
    }
}