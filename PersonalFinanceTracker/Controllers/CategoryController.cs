using PersonalFinanceTracker.Models.ViewModels;
using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace PersonalFinanceTracker.Controllers
{
    public class CategoryController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CategoryController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44356/api/");
        }

        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        // GET: Category/ListCategories
        public ActionResult ListCategories()
        {
            ListCategories viewModel = new ListCategories();

            // Get Income Categories
            string url = "CategoryData/listCategoryByTransactionType?transactionTypeName=Income";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                viewModel.IncomeCategoryList = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;
            }

            // Get Expense Categories
            url = "CategoryData/listCategoryByTransactionType?transactionTypeName=Expense";
            response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                viewModel.ExpenseCategoryList = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;
            }

            return View(viewModel);
        }

        // POST: Category/AddCategory
        [HttpPost]
        public ActionResult AddCategory(Category category)
        {
            string url = "CategoryData/addCategory";

            string jsonpayload = jss.Serialize(category);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListCategories");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while adding the category. Please try again.";
                return RedirectToAction("ListCategories");
            }
        }

        // POST: Category/UpdateCategory
        [HttpPost]
        public ActionResult UpdateCategory(Category category)
        {
            string url = $"CategoryData/updateCategory/{category.CategoryId}";
            string jsonpayload = jss.Serialize(category);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PutAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListCategories");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while updating the category. Please try again.";
                return RedirectToAction("ListCategories");
            }
        }

        // POST: Category/DeleteCategory
        [HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            string url = $"CategoryData/deleteCategory/{categoryId}";
            HttpResponseMessage response = client.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListCategories");
            }
            else
            {
                TempData["ErrorMessage"] = "This Category cannot be deleted as it has associated Transactions";
                TempData["BackUrl"] = Url.Action("ListCategories", "Category");
                return RedirectToAction("Error");
            }
        }

        //GET: Transaction/Error
        public ActionResult Error()
        {
            return View();
        }

    }
}