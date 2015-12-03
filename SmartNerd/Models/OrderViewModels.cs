using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartNerd.Models.Order {
    public class Order {
        public int OrderID { get; set; }
        public decimal Total { get; set; }
        public DateTime DatePlaced { get; set; }
        public Address Address { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }

    public class Product {
        public int ProductID { get; set; }
        public String Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}