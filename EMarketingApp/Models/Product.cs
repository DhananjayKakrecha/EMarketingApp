using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EMarketingApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1,1000000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        public string ProductImage { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int AnalystId { get; set; }
        public virtual Analyst Analyst { get; set; }

        // New fields
        public DateTime? DisplayUntil { get; set; } // Nullable in case it's not set
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }    // Stores the email address for the product

        public int? Likes   { get; set; }
        public int? Dislikes { get; set; }
    }
}