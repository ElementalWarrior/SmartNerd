using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Models.Admin {
    public class Category {
        public int CategoryID { get; set; }
        public String CategoryName { get; set; }
    }
    public class Product {
        public int ProductID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string ProductName { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
    }
    public class BrowsePage {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
    }

    public class SearchPage {
        public List<Product> Products { get; set; }
        public String SearchTerm { get; set; }
    }
    public class ReportEntry
    {
        public decimal DailyTotal { get; set; }
        public int NumberOfOrders { get; set; }
        public DateTime DatePlaced { get; set; }
    }
    public class FrequentEntry
    {
        public decimal TotalRevenue { get; set; }
        public int NumberOrdered { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; }
    }
    public class ReportPage
    {
        public List<ReportEntry> DailyReport { get; set; }
        public List<FrequentEntry> FrequentProducts { get; set; }
    }
}