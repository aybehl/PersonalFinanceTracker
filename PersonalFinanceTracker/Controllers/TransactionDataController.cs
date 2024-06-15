using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PersonalFinanceTracker.Models.ViewModels;
using System.Collections;

namespace PersonalFinanceTracker.Controllers
{
    
    public class TransactionDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// This method will access the local database to get all the Transactions from the Transactions table
        /// </summary>
        /// <example>
        /// GET api/TransactionData/ListAllTransactions -> [{"TransactionId":1,"Title":"Bought Ice-cream and some fruits ","Amount":12.99,"TransactionDate":"2024-06-05T00:00:00","CategoryName":"Groceries","TransactionTypeName":"Expense"},{"TransactionId":2,"Title":"May Salary","Amount":4500.00,"TransactionDate":"2024-06-01T00:00:00","CategoryName":"Salary","TransactionTypeName":"Income"}]
        /// </example>
        /// <returns>A list of all Transactions</returns>        
        [HttpGet]
        [Route("api/TransactionData/ListAllTransactions")]
        public IEnumerable<TransactionDto> ListAllTransactions(bool currentMonth = false)
        {
            var transactions = db.Transactions
                .Include(t => t.Category)
                .Include(t => t.Category.TransactionType)
                .AsQueryable();

            if (currentMonth)
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
                transactions = transactions.Where(t => t.TransactionDate >= startOfMonth && t.TransactionDate < endOfMonth);
            }

            var transactionList = transactions.ToList();

            List<TransactionDto> transactionDtos = new List<TransactionDto>();

            foreach (Transaction transaction in transactionList)
            {
                TransactionDto transactionDto = new TransactionDto
                {
                    TransactionId = transaction.TransactionId,
                    Title = transaction.Title,
                    Amount = transaction.Amount,
                    CategoryName = transaction.Category.CategoryName,
                    TransactionDate = transaction.TransactionDate,
                    TransactionTypeName = transaction.Category.TransactionType.TransactionTypeName
                };

                transactionDtos.Add(transactionDto);
            }

            return transactionDtos;
        }

        /// <summary>
        /// This method will access the local database to get the Transaction from the Transactions table for the given Id
        /// </summary>
        /// <example>
        /// GET api/TransactionData/findTransactionById/1 -> {"TransactionId":1,"Title":"Bought Ice-cream and some fruits ","Amount":12.99,"TransactionDate":"2024-06-05T00:00:00","CategoryName":"Groceries","TransactionTypeName":"Expense"},{"TransactionId":2,"Title":"May Salary","Amount":4500.00,"TransactionDate":"2024-06-01T00:00:00","CategoryName":"Salary","TransactionTypeName":"Income"}
        /// </example>
        /// <returns>A Transaction</returns>        
        [HttpGet]
        [Route("api/TransactionData/findTransactionById/{transactionId}")]
        public IHttpActionResult findTransactionById(int transactionId)
        {
            Transaction transaction = db.Transactions.Find(transactionId);
            if (transaction == null) {
                return NotFound();
            }
            
            TransactionDto transactionDto = new TransactionDto();

            transactionDto.TransactionId = transaction.TransactionId;
            transactionDto.Title = transaction.Title;
            transactionDto.Amount = transaction.Amount;
            transactionDto.CategoryName = transaction.Category.CategoryName;
            transactionDto.TransactionDate = transaction.TransactionDate;
            transactionDto.TransactionTypeName = transaction.Category.TransactionType.TransactionTypeName;

            return Ok(transactionDto);
        }

        /// <summary>
        /// This method will access the local database to get all the Transactions from the Transactions table that match the filters provided as query parameters
        /// Filter Types:
        /// 1. Transaction Id
        /// 2. Category Name
        /// 3. Transaction Type
        /// 4. current Month
        /// 5. last Month
        /// </summary>
        /// <example>
        /// GET api/TransactionData/findTransactions -> [{"TransactionId":1,"Title":"Bought Ice-cream and some fruits ","Amount":12.99,"TransactionDate":"2024-06-05T00:00:00","CategoryName":"Groceries","TransactionTypeName":"Expense"},{"TransactionId":2,"Title":"May Salary","Amount":4500.00,"TransactionDate":"2024-06-01T00:00:00","CategoryName":"Salary","TransactionTypeName":"Income"}]
        /// Find by Id: GET /api/TransactionData/findTransactions?id=1
        /// Find by Category Name: GET /api/TransactionData/findTransactions?categoryName=Groceries
        /// Find by Transaction Type: GET /api/TransactionData/findTransactions?transactionType=Income
        /// Current Month Transactions: GET /api/TransactionData/findTransactions?currentMonth=true
        /// Last Month Transactions: GET /api/TransactionData/findTransactions?lastMonth=true
        /// Combined filters: GET /api/TransactionData/findTransactions?categoryName=Groceries&transactionType=Expense&currentMonth=true
        /// </example>
        /// <returns>A list of all Transactions that match the filter</returns>
        [HttpGet]
        [Route("api/TransactionData/findTransactions")]
        public IHttpActionResult FindTransactions([FromUri] int? id = null, [FromUri] string categoryName = null, [FromUri] string transactionType = null, [FromUri] bool currentMonth = false, [FromUri] bool lastMonth = false)
        {
            var query = db.Transactions.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(t => t.TransactionId == id.Value);
            }

            if (!string.IsNullOrEmpty(categoryName))
            { 
                query = query.Where(t => t.Category.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(t => t.Category.TransactionType.TransactionTypeName.Equals(transactionType, StringComparison.OrdinalIgnoreCase));
            }

            if (currentMonth)
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);

                query = query.Where(t => t.TransactionDate >= startOfMonth && t.TransactionDate < endOfMonth);
            }

            if (lastMonth)
            {
                var startOfLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                var endOfLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                query = query.Where(t => t.TransactionDate >= startOfLastMonth && t.TransactionDate < endOfLastMonth);
            }

            var transactionList = query.ToList();

            List<TransactionDto> transactionDtos = new List<TransactionDto>();

            foreach (Transaction transaction in transactionList)
            {
                TransactionDto transactionDto = new TransactionDto();

                transactionDto.TransactionId = transaction.TransactionId;
                transactionDto.Title = transaction.Title;
                transactionDto.Amount = transaction.Amount;
                transactionDto.CategoryName = transaction.Category.CategoryName;
                transactionDto.TransactionDate = transaction.TransactionDate;
                transactionDto.TransactionTypeName = transaction.Category.TransactionType.TransactionTypeName;

                transactionDtos.Add(transactionDto);
            }


            return Ok(transactionDtos);
        }

        /// <summary>
        /// This api helps add a new Transaction to the Transactions Table in the DB
        /// </summary>
        /// <param name="transaction">JSON FORM DATA of a Transaction</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: TransactionID, Transaction Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        ///curl -d @C:\Users\Ahzi\source\repos\PersonalFinanceTracker\PersonalFinanceTracker\JsonData\Transaction.json -H "Content-Type:application/json" https://localhost:44356/api/TransactionData/addTransaction
        ///{"TransactionId":9,"Title":"Cheque for freelance software gig","Amount":1000.99,"TransactionDate":"2024-06-03T00:00:00","CategoryId":2,"Category":null}
        /// </example>
        [ResponseType(typeof(Transaction))]
        [HttpPost]
        [Route("api/TransactionData/AddTransaction")]
        public IHttpActionResult AddTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Transactions.Add(transaction);
            db.SaveChanges();

            // Manually construct the URL for the newly created resource
            //var location = new Uri(Request.RequestUri, $"/api/TransactionData/{transaction.TransactionId}");

            // Return a response with the location of the newly created transaction
            //return Created(location, transaction);

            return Ok();
        }

        /// <summary>
        /// Deletes a transaction from the system by its ID
        /// </summary>
        /// <param name="id">The primary key of the transaction</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// DELETE: api/TransactionData/DeleteTransaction/5
        /// FORM DATA: (empty)
        /// curl -i -X DELETE https://localhost:44356/api/TransactionData/deleteTransaction/7
        /// HTTP/1.1 200 OK
        /// </example>
        [ResponseType(typeof(Transaction))]
        [HttpDelete]
        [Route("api/TransactionData/DeleteTransaction/{id}")]
        public IHttpActionResult DeleteTransaction(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();

            return Ok(transaction);
        }

        /// <summary>
        /// Updates a particular transaction in the system with PUT Data input
        /// </summary>
        /// <param name="id">Represents the Transaction ID primary key</param>
        /// <param name="transaction">JSON FORM DATA of a transaction</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// PUT: api/TransactionData/UpdateTransaction/5
        /// FORM DATA: Transaction JSON Object
        /// curl -X PUT -d @C:\Users\Ahzi\source\repos\PersonalFinanceTracker\PersonalFinanceTracker\JsonData\Transaction.json -H "Content-Type:application/json" https://localhost:44356/api/TransactionData/updateTransaction/2
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/TransactionData/UpdateTransaction/{id}")]
        public IHttpActionResult UpdateTransaction(int id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            db.Entry(transaction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        private bool TransactionExists(int id) {
            return db.Transactions.Count(t => t.TransactionId == id) > 0;
        }

        /// <summary>
        /// Gets the total amount spent or gained per category.
        /// </summary>
        /// <example> 
        /// GET api/TransactionData/CategoryTotals -> [{"CategoryName":"Groceries","CategoryId":1,"TransactionType":"Expense","TotalAmount":80.65},{"CategoryName":"Salary","CategoryId":2,"TransactionType":"Income","TotalAmount":5500.99},{"CategoryName":"Bonus","CategoryId":11,"TransactionType":"Income","TotalAmount":500.00}]
        /// </example> 
        /// <returns>A list of category totals grouped by transaction type and category name.</returns>
        [HttpGet]
        [Route("api/TransactionData/CategoryTotals")]
        public IEnumerable<CategoryTotal> GetCategoryTotals(bool currentMonth = false)
        {
            var transactions = db.Transactions
                .Include(t => t.Category)
                .Include(t => t.Category.TransactionType)
                .AsQueryable();

            if (currentMonth)
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
                transactions = transactions.Where(t => t.TransactionDate >= startOfMonth && t.TransactionDate < endOfMonth);
            }
            
            var categoryTotals = transactions
                .GroupBy(t => new { t.Category.CategoryName, t.Category.CategoryId, t.Category.TransactionType.TransactionTypeName })
                .Select(g => new CategoryTotal
                {
                    TransactionType = g.Key.TransactionTypeName,
                    CategoryName = g.Key.CategoryName,
                    CategoryId = g.Key.CategoryId,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .ToList();

            return categoryTotals;
        }

        /// <summary>
        /// Gets the total amount spent or gained per transaction type (Income or Expense).
        /// </summary>
        /// <example>
        /// GET api/TransactionData/TransactionTypeTotals -> [{"TransactionTypeName":"Expense","TransactionTypeId":0,"TotalAmount":80.65},{"TransactionTypeName":"Income","TransactionTypeId":0,"TotalAmount":6000.99}]
        /// </example>
        /// <returns>A list of totals grouped by transaction type.</returns>
        [HttpGet]
        [Route("api/TransactionData/TransactionTypeTotals")]
        public IEnumerable<TransactionTypeTotal> GetTransactionTypeTotals(bool currentMonth = false)
        {
            var transactions = db.Transactions
                .Include(t => t.Category.TransactionType)
                .AsQueryable();

            if (currentMonth)
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
                transactions = transactions.Where(t => t.TransactionDate >= startOfMonth && t.TransactionDate < endOfMonth);
            }

            var transactionTypeTotals = transactions
                .GroupBy(t => new { t.Category.TransactionType.TransactionTypeName, t.Category.TransactionType.TransactionTypeId })
                .Select(g => new TransactionTypeTotal
                {
                    TransactionTypeId = g.Key.TransactionTypeId,
                    TransactionTypeName = g.Key.TransactionTypeName,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .ToList();

            return transactionTypeTotals;
        }
    }
}

