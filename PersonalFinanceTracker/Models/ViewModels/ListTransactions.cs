using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models.ViewModels
{
    public class ListTransactions
    {
        public IEnumerable<TransactionDto> TransactionList { get; set; }

        public IEnumerable<CategoryDto> CategoryList { get; set; }

        public string SelectedFilter { get; set; }
        public string SelectedCategory { get; set; }
    }
}