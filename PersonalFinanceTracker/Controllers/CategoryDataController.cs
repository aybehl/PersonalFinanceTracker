using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Caching;

namespace PersonalFinanceTracker.Controllers
{
    public class CategoryDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
                categoryDto.TransactionTypeId = category.TransactionType.TransactionTypeId;
                categoryDtos.Add(categoryDto);
            }

            return Ok(categoryDtos);
        }

        /// <summary>
        /// This api helps add a new Category to the Categories Table in the DB
        /// </summary>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Category Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        ///curl -d @C:\Users\Ahzi\source\repos\PersonalFinanceTracker\PersonalFinanceTracker\JsonData\Category.json -H "Content-Type:application/json" https://localhost:44356/api/CategoryData/addCategory
        /// {"CategoryName": "Testing","TransactionTypeId": 1,"TransactionType": null}
        /// </example>
        [HttpPost]
        [ResponseType(typeof(Category))]
        [Route("api/CategoryData/addCategory")]
        public IHttpActionResult AddCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Categories.Add(category);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Updates a particular Category in the system with PUT Data input
        /// </summary>
        /// <param name="id">Represents the Category ID primary key</param>
        /// <param name="Category">JSON FORM DATA of a Category</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// PUT: api/CategoryData/UpdateCategory/5
        /// FORM DATA: Category JSON Object
        /// curl -i -X PUT -d @C:\Users\Ahzi\source\repos\PersonalFinanceTracker\PersonalFinanceTracker\JsonData\Category.json -H "Content-Type:application/json" https://localhost:44356/api/CategoryData/updateCategory/16 
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/CategoryData/UpdateCategory/{id}")]
        public IHttpActionResult UpdateCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(c => c.CategoryId == id) > 0;
        }

        /// <summary>
        /// Deletes a Category from the system by its ID
        /// </summary>
        /// <param name="id">The primary key of the Category</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// DELETE: api/CategoryData/DeleteCategory/5
        /// FORM DATA: (empty)
        /// >curl -i -X DELETE https://localhost:44356/api/CategoryData/deleteCategory/16
        ///{"TransactionType":null,"CategoryId":16,"CategoryName":"Testing2","TransactionTypeId":2}
        /// HTTP/1.1 200 OK
        /// </example>
        [HttpDelete]
        [ResponseType(typeof(Category))]
        [Route("api/CategoryData/DeleteCategory/{id}")]
        public IHttpActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            if (db.Transactions.Any(t => t.CategoryId == id))
            {
                return Content(HttpStatusCode.Conflict, "Category cannot be deleted because it has associated transactions.");
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return Ok(category);
        }

    }
}
