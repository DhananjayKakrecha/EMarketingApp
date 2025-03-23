using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMarketingApp.Models
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; } // Product details
        public IEnumerable<Feedback> Feedbacks { get; set; } // Related feedback
        public bool IsAddedToMainSite { get; set; }
    }
}