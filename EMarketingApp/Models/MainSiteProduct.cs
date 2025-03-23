using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMarketingApp.Models
{
    public class MainSiteProduct
    {
        public int MainSiteProductId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}