using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartNerd
{
    public class Payment
    {
        public String CardType { get; set; }
        public String CardNumber { get; set; }
        public String LastFour { get; set; }
        public String PayPalID { get; set; }
        public Decimal Amount { get; set; }
    }
}