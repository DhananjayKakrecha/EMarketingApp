using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMarketingApp.Models
{
    public class DemoPageViewModel
    {
        public IEnumerable<Category>   Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}