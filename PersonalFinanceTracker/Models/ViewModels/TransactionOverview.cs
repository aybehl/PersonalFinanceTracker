using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models.ViewModels
{
    public class TransactionOverview
    {
        public IEnumerable<CategoryTotal> CategoryTotalList { get; set; }

        public IEnumerable<TransactionTypeTotal> TransactionTypeTotalList { get; set; }
        public IEnumerable<TransactionDto> TransactionList { get; set; }

        public String CurrentMonth { get; set; }
    }
}