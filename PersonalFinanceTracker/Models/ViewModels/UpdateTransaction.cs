using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models.ViewModels
{
    public class UpdateTransaction
    {
        public TransactionDto SelectedTransaction { get; set; }

        public IEnumerable<CategoryDto> CategoryOptions { get; set; }
    }
}