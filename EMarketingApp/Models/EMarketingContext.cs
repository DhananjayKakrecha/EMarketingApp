using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace EMarketingApp.Models
{
    public class EMarketingContext : DbContext
    {
        public EMarketingContext() : base("name=EMarketingContext")
        {

        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Analyst>   Analysts { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Marketer> Marketers { get; set; }
        public DbSet<Vote> Votes { get; set; } 
        public DbSet<Feedback>  Feedbacks { get; set; }

        public DbSet<MainSiteProduct> MainSiteProducts { get; set; }
    }
}