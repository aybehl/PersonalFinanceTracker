using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PersonalFinanceTracker.Controllers
{
    public class CategoryDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// This method will access the local database to get all the Categories from the Categoriess table for a TransactionType
        /// </summary>
        /// <example>
        /// GET api/CategoryData/findCategoryByTransactionType
        /// curl https://localhost:44356/api/CategoryData/findCategoryByTransactionType?transactionTypeId=1
        /// [{"CategoryId":2,"CategoryName":"Salary","TransactionTypeName":"Income"}]
        /// </example>
        /// <returns>Category Objects</returns>        
        [HttpGet]
        [ResponseType(typeof(Category))]
        [Route("api/CategoryData/findCategoryByTransactionType")]
        public IHttpActionResult findCategoryByTransactionType(int transactionTypeId)
        {
            List<Category> categoryList = db.Categories
                .Where(c => c.TransactionTypeId == transactionTypeId)
                .ToList();
            if (categoryList.Count == 0) {
                return NotFound();
            }

            List<CategoryDto> categoryDtos = new List<CategoryDto> ();

            foreach (var category in categoryList)
            {
                CategoryDto categoryDto = new CategoryDto();

                categoryDto.CategoryName = category.CategoryName;
                categoryDto.CategoryId = category.CategoryId;
                categoryDto.TransactionTypeName = category.TransactionType.TransactionTypeName;

                categoryDtos.Add(categoryDto);
            }

            return Ok(categoryDtos);
        }

        /// <summary>
        /// This method will access the local database to get all the Categories from the Categoriess table for a TransactionTypeName
        /// </summary>
        /// <example>
        /// GET api/CategoryData/listCategoryByTransactionType?transactionTypeName=Income
        /// [{"CategoryId":2,"CategoryName":"Salary","TransactionTypeName":"Income"}]
        /// </example>
        /// <returns>Category Objects</returns>        
        [HttpGet]
        [ResponseType(typeof(Category))]
        [Route("api/CategoryData/listCategoryByTransactionType")]
        public IHttpActionResult listCategoryByTransactionType(string transactionTypeName)
        {
            List<Category> categoryList = db.Categories
                .Where(c => c.TransactionType.TransactionTypeName == transactionTypeName)
                .ToList();

            if (categoryList.Count == 0)
            {
                return NotFound();
            }

            List<CategoryDto> categoryDtos = new List<CategoryDto>();

            foreach (var category in categoryList)
            {
                CategoryDto categoryDto = new CategoryDto();

                categoryDto.CategoryName = category.CategoryName;
                categoryDto.CategoryId = category.CategoryId;
                categoryDto.TransactionTypeName = category.TransactionType.TransactionTypeName;

                categoryDtos.Add(categoryDto);
            }

            return Ok(categoryDtos);
        }

    }
}
