using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Models.Admin
{
    public class Category
    {
        public int CategoryID { get; set; }
        public String CategoryName { get; set; }
    }
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
    }
    public class BrowsePage
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
    }

    public class SearchPage {
        public List<Product> Products { get; set; }
        public String SearchTerm { get; set; }
    }
}