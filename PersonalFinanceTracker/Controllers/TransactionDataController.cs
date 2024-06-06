using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

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
        public IEnumerable<TransactionDto> ListAllTransactions() {
            List<Transaction> transactionList = db.Transactions.ToList();

            List<TransactionDto> transactionDtos = new List<TransactionDto>();

            foreach (Transaction transaction in transactionList) { 
                TransactionDto transactionDto = new TransactionDto();

                transactionDto.TransactionId = transaction.TransactionId;
                transactionDto.Title = transaction.Title;
                transactionDto.Amount = transaction.Amount;
                transactionDto.CategoryName = transaction.Category.CategoryName;
                transactionDto.TransactionDate = transaction.TransactionDate;
                transactionDto.TransactionTypeName = transaction.Category.TransactionType.TransactionTypeName;

                transactionDtos.Add(transactionDto);
            }

            return transactionDtos;
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

            return CreatedAtRoute("DefaultApi", new { id = transaction.TransactionId }, transaction);
        }


    }
}

