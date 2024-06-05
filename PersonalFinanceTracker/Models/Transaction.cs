using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        //[ForeignKey("TransactionType")]
        //public int TransactionTypeId { get; set; }
        //public virtual TransactionType TransactionType { get; set; }
    }
}