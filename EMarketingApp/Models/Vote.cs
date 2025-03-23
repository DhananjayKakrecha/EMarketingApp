using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMarketingApp.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int MarketerId   { get; set; }
        [Required]
        public string Action   { get; set; }
    }
}