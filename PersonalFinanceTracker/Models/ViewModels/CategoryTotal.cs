using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models.ViewModels
{
    public class CategoryTotal
    {
        public String CategoryName { get; set; }
        public int CategoryId { get; set; }
        public String TransactionType { get; set; }
        public decimal TotalAmount { get; set; }
    }
}