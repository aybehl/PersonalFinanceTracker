using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        [ForeignKey("TransactionType")]
        public int TransactionTypeId { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }

    public class CategoryDto {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string TransactionTypeName { get; set; }
        public int TransactionTypeId { get; set; }
    }
}