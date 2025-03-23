using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EMarketingApp.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int MarketerId { get; set; }
        [Required]
        public string FeedbackText { get; set; }

        public virtual Marketer Marketer { get; set; }
        public virtual Product Product { get; set; }
    }
}