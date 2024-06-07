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
            return View();
        }

        //GET: Transaction/ListExpenses
        public ActionResult ListExpenses() {
            string url = "TransactionData/findTransactions?transactionType=Expense";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<TransactionDto> expenseTransactions = response.Content.ReadAsAsync<IEnumerable<TransactionDto>>().Result;

            return View(expenseTransactions);        
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

        // POST: Transaction/CreateExpense
        [HttpPost]
        //[Route("/Transaction/CreateExpense")]
        public ActionResult CreateExpense(Transaction transaction)
        {
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

        // POST: Transaction/UpdateExpense/2
        [HttpPost]
        public ActionResult UpdateExpense(int id, Transaction transaction)
        {

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

        // POST: /Transaction/DeleteExpense/id
        [HttpPost]
        public ActionResult DeleteExpense(int id)
        {
            string url = "TransactionData/DeleteTransaction" + id;
            
            HttpResponseMessage response = client.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred when deleting an expense. Please try again.";
                TempData["BackUrl"] = Url.Action("ListExpenses", "Transaction");
                return RedirectToAction("Error");
            }
        }

        //GET: Transaction/Error
        public ActionResult Error() {
            return View();
        }
    }
}