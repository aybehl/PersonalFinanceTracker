using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace PersonalFinanceTracker.Controllers
{
    public class TransactionController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static TransactionController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44356/api/");
        }

        // GET: Transaction
        public ActionResult Index()
        {   
            TransactionOverview TransactionOverview = new TransactionOverview();

            //Get all Transactions
            string url = "TransactionData/ListAllTransactions?currentMonth=true";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred while listing all Transactions. Please try again.";
                TempData["BackUrl"] = "/";
                return RedirectToAction("Error");
            }

            IEnumerable<TransactionDto> transactions = response.Content.ReadAsAsync<IEnumerable<TransactionDto>>().Result;
            TransactionOverview.TransactionList = transactions;

            //Get total amount spent or gained per Category
            url = "TransactionData/CategoryTotals?currentMonth=true";
            response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred when getting Total Amount spent or gained per category. Please try again.";
                TempData["BackUrl"] = "/";
                return RedirectToAction("Error");
            }

            IEnumerable<CategoryTotal> CategoryTotals = response.Content.ReadAsAsync<IEnumerable<CategoryTotal>>().Result;
            TransactionOverview.CategoryTotalList = CategoryTotals;

            //Get total amount spent or gained per TransactionType
            url = "TransactionData/TransactionTypeTotals?currentMonth=true";
            response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred when getting Total Amount spent or gained per TransactionType. Please try again.";
                TempData["BackUrl"] = "/";
                return RedirectToAction("Error");
            }

            IEnumerable<TransactionTypeTotal> TransactionTypeTotalList = response.Content.ReadAsAsync<IEnumerable<TransactionTypeTotal>>().Result;
            TransactionOverview.TransactionTypeTotalList = TransactionTypeTotalList;

            // Get the name of the current month
            DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            TransactionOverview.CurrentMonth = startOfMonth.ToString("MMMM");

            return View(TransactionOverview);
        }

        //GET: Transaction/ListExpenses
        public ActionResult ListExpenses(string filter = null, string categoryName = null)
        {
            ListTransactions ViewModel = new ListTransactions();

            //Get all categories for Expenses to render the dowpdown
            string url = "categoryData/listCategoryByTransactionType?transactionTypeName=Expense";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred while listing all expense Categories. Please try again.";
                TempData["BackUrl"] = Url.Action("ListExpenses", "Transaction");
                return RedirectToAction("Error");
            }

            IEnumerable<CategoryDto> categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;
            ViewModel.CategoryList = categories;

            url = "TransactionData/findTransactions?transactionType=Expense";
            
            if (filter == "currentMonth")
            {
                url += "&currentMonth=true";
            }
            else if (filter == "lastMonth")
            {
                url += "&lastMonth=true";
            }
            
            if (!string.IsNullOrEmpty(categoryName))
            {
                url += $"&categoryName={categoryName}";
            }

            response = client.GetAsync(url).Result;
            IEnumerable<TransactionDto> expenseTransactions = response.Content.ReadAsAsync<IEnumerable<TransactionDto>>().Result;
            ViewModel.TransactionList = expenseTransactions;
            ViewModel.SelectedFilter = filter;
            ViewModel.SelectedCategory = categoryName;

            return View(ViewModel);
        }

        //GET: Transaction/ListIncomes
        public ActionResult ListIncomes(string filter = null, string categoryName = null)
        {
            ListTransactions ViewModel = new ListTransactions();

            //Get all categories for Incomes to render the dowpdown
            string url = "categoryData/listCategoryByTransactionType?transactionTypeName=Income";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred while listing all income Categories. Please try again.";
                TempData["BackUrl"] = Url.Action("ListIncomes", "Transaction");
                return RedirectToAction("Error");
            }

            IEnumerable<CategoryDto> categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;
            ViewModel.CategoryList = categories;

            url = "TransactionData/findTransactions?transactionType=Income";

            if (filter == "currentMonth")
            {
                url += "&currentMonth=true";
            }
            else if (filter == "lastMonth")
            {
                url += "&lastMonth=true";
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                url += $"&categoryName={categoryName}";
            }

            response = client.GetAsync(url).Result;
            IEnumerable<TransactionDto> incomeTransactions = response.Content.ReadAsAsync<IEnumerable<TransactionDto>>().Result;
            ViewModel.TransactionList = incomeTransactions;
            ViewModel.SelectedFilter = filter;
            ViewModel.SelectedCategory = categoryName;

            return View(ViewModel);
        }

        // GET: Transaction/NewExpense
        public ActionResult NewExpense()
        {
            //Get all categories for Expenses to render the dowpdown
            string url = "categoryData/listCategoryByTransactionType?transactionTypeName=Expense";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<CategoryDto> categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;

            return View(categories);
        }

        // GET: Transaction/NewIncome
        public ActionResult NewIncome()
        {
            //Get all categories for Incomes to render the dowpdown
            string url = "categoryData/listCategoryByTransactionType?transactionTypeName=Income";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<CategoryDto> categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;

            return View(categories);
        }

        // POST: Transaction/CreateExpense
        [HttpPost]
        public ActionResult CreateExpense(Transaction transaction)
        {
            transaction.TransactionDate = DateTime.SpecifyKind(transaction.TransactionDate, DateTimeKind.Utc);
            string url = "TransactionData/AddTransaction";

            string jsonpayload = jss.Serialize(transaction);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListExpenses");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while adding an expense. Please try again.";
                TempData["BackUrl"] = Url.Action("NewExpense", "Transaction");
                return RedirectToAction("Error");
            }
        }

        // POST: Transaction/CreateIncome
        [HttpPost]
        public ActionResult CreateIncome(Transaction transaction)
        {
            transaction.TransactionDate = DateTime.SpecifyKind(transaction.TransactionDate, DateTimeKind.Utc);

            string url = "TransactionData/AddTransaction";

            string jsonpayload = jss.Serialize(transaction);
            
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListIncomes");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while adding an Income. Please try again.";
                TempData["BackUrl"] = Url.Action("NewIncome", "Transaction");
                return RedirectToAction("Error");
            }
        }

        // GET: Transaction/EditExpense/5
        public ActionResult EditExpense(int id)
        {
            UpdateTransaction ViewModel = new UpdateTransaction();

            //Get the transaction details with the given transactionid
            string url = "TransactionData/findTransactionById/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode) {
                TempData["ErrorMessage"] = "An error occurred while finding an expense. Please try again.";
                TempData["BackUrl"] = Url.Action("ListExpenses", "Transaction");
                return RedirectToAction("Error");
            }

            TransactionDto selectedTransaction = response.Content.ReadAsAsync<TransactionDto>().Result;
            ViewModel.SelectedTransaction = selectedTransaction;

            //Get all categories for Expenses to render the dowpdown
            url = "categoryData/listCategoryByTransactionType?transactionTypeName=Expense";
            response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred while listing all expense Categories. Please try again.";
                TempData["BackUrl"] = Url.Action("ListExpenses", "Transaction");
                return RedirectToAction("Error");
            }

            IEnumerable<CategoryDto> categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;

            ViewModel.CategoryOptions = categories;

            return View(ViewModel);
        }

        // GET: Transaction/EditIncome/5
        public ActionResult EditIncome(int id)
        {
            UpdateTransaction ViewModel = new UpdateTransaction();

            //Get the transaction details with the given transactionid
            string url = "TransactionData/findTransactionById/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred while finding an Income. Please try again.";
                TempData["BackUrl"] = Url.Action("ListIncomes", "Transaction");
                return RedirectToAction("Error");
            }

            TransactionDto selectedTransaction = response.Content.ReadAsAsync<TransactionDto>().Result;
            ViewModel.SelectedTransaction = selectedTransaction;

            //Get all categories for Incomes to render the dowpdown
            url = "categoryData/listCategoryByTransactionType?transactionTypeName=Income";
            response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "An error occurred while listing all Income Categories. Please try again.";
                TempData["BackUrl"] = Url.Action("ListIncomes", "Transaction");
                return RedirectToAction("Error");
            }

            IEnumerable<CategoryDto> categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;

            ViewModel.CategoryOptions = categories;

            return View(ViewModel);
        }

        // POST: Transaction/UpdateExpense/2
        [HttpPost]
        public ActionResult UpdateExpense(int id, Transaction transaction)
        {
            transaction.TransactionDate = DateTime.SpecifyKind(transaction.TransactionDate, DateTimeKind.Utc);

            string url = "TransactionData/UpdateTransaction/" + id;
            transaction.TransactionId = id;
            string jsonpayload = jss.Serialize(transaction);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PutAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListExpenses");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred when updating an expense. Please try again.";
                TempData["BackUrl"] = Url.Action("ListExpenses", "Transaction");
                return RedirectToAction("Error");
            }
        }

        // POST: Transaction/UpdateIncome/2
        [HttpPost]
        public ActionResult UpdateIncome(int id, Transaction transaction)
        {
            transaction.TransactionDate = DateTime.SpecifyKind(transaction.TransactionDate, DateTimeKind.Utc);

            string url = "TransactionData/UpdateTransaction/" + id;
            transaction.TransactionId = id;
            string jsonpayload = jss.Serialize(transaction);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PutAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListIncomes");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred when updating an Income. Please try again.";
                TempData["BackUrl"] = Url.Action("ListIncomes", "Transaction");
                return RedirectToAction("Error");
            }
        }

        // POST: /Transaction/DeleteExpense/id
        [HttpPost]
        public ActionResult DeleteExpense(int id)
        {
            string url = "TransactionData/DeleteTransaction" + id;
            
            HttpResponseMessage response = client.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListExpenses");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred when deleting an expense. Please try again.";
                TempData["BackUrl"] = Url.Action("ListExpenses", "Transaction");
                return RedirectToAction("Error");
            }
        }

        // POST: /Transaction/DeleteIncome/id
        [HttpPost]
        public ActionResult DeleteIncome(int id)
        {
            string url = "TransactionData/DeleteTransaction" + id;

            HttpResponseMessage response = client.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListIncomes");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred when deleting an Income. Please try again.";
                TempData["BackUrl"] = Url.Action("ListIncomes", "Transaction");
                return RedirectToAction("Error");
            }
        }

        //GET: Transaction/Error
        public ActionResult Error() {
            return View();
        }
    }
}