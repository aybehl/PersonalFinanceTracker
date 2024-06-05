using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models
{
    public class TransactionType
    {
        [Key]
        public int TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
    }
}