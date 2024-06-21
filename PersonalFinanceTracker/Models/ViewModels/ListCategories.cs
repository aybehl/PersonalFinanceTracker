using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceTracker.Models.ViewModels
{
    public class ListCategories
    {
        public IEnumerable<CategoryDto> ExpenseCategoryList { get; set; }
        public IEnumerable<CategoryDto> IncomeCategoryList { get; set; }
    }
}