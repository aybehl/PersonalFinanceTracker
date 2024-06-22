# PersonalFinanceTracker

This repository contains the code for a Personal Finance Tracker called SpendSmart. It is created using the ASP.NET framework.

## Entities

### Transaction
- `TransactionId` - Primary key
- `Title` - Title of the transaction
- `Amount` - Amount of the transaction
- `TransactionDate` - Date of the transaction
- `CategoryId` - Foreign key to the Category entity

### Category
- `CategoryId` - Primary key
- `CategoryName` - Name of the category (e.g., Salary, Groceries)
- `TransactionTypeId` - Foreign key to the TransactionType entity

### TransactionType
- `TransactionTypeId` - Primary key
- `TransactionTypeName` - Type of the transaction (e.g., Expense, Income)

## Data Controllers

### TransactionDataController

#### ListAllTransactions
- **URL**: `api/TransactionData/ListAllTransactions`
- **Method**: GET
- **Description**: Retrieves a list of all transactions. Optionally filters transactions for the current month.
- **Parameters**:
  - `currentMonth` (optional, bool)
- **Example**: `GET api/TransactionData/ListAllTransactions?currentMonth=true`

#### findTransactionById
- **URL**: `api/TransactionData/findTransactionById/{transactionId}`
- **Method**: GET
- **Description**: Finds a transaction by its ID.
- **Parameters**: `transactionId` (int)
- **Example**: `GET api/TransactionData/findTransactionById/1`

#### findTransactions
- **URL**: `api/TransactionData/findTransactions`
- **Method**: GET
- **Description**: Finds transactions based on optional filters such as category name, transaction type, current month, or last month.
- **Parameters**:
  - `id` (optional, int)
  - `categoryName` (optional, string)
  - `transactionType` (optional, string)
  - `currentMonth` (optional, bool)
  - `lastMonth` (optional, bool)
- **Examples**:
  - `GET api/TransactionData/findTransactions`
  - `GET api/TransactionData/findTransactions?id=1`
  - `GET api/TransactionData/findTransactions?categoryName=Groceries`
  - `GET api/TransactionData/findTransactions?transactionType=Income`
  - `GET api/TransactionData/findTransactions?currentMonth=true`
  - `GET api/TransactionData/findTransactions?lastMonth=true`
  - `GET api/TransactionData/findTransactions?categoryName=Groceries&transactionType=Expense&currentMonth=true`

#### AddTransaction
- **URL**: `api/TransactionData/AddTransaction`
- **Method**: POST
- **Description**: Adds a new transaction.
- **Parameters**: `transaction` (Transaction JSON object)

#### DeleteTransaction
- **URL**: `api/TransactionData/DeleteTransaction/{id}`
- **Method**: DELETE
- **Description**: Deletes a transaction from the system by its ID.
- **Parameters**: `id` (int)

#### UpdateTransaction
- **URL**: `api/TransactionData/UpdateTransaction/{id}`
- **Method**: PUT
- **Description**: Updates a particular transaction in the system with PUT data input.
- **Parameters**:
  - `id` (int)
  - `transaction` (Transaction JSON object)

### CategoryDataController

#### listCategoryByTransactionType
- **URL**: `api/CategoryData/listCategoryByTransactionType`
- **Method**: GET
- **Description**: Retrieves a list of categories based on the transaction type name.
- **Parameters**: `transactionTypeName` (string)
- **Example**: `GET api/CategoryData/listCategoryByTransactionType?transactionTypeName=Income`

#### AddCategory
- **URL**: `api/CategoryData/AddCategory`
- **Method**: POST
- **Description**: Adds a new category.
- **Parameters**: `category` (Category JSON object)

#### UpdateCategory
- **URL**: `api/CategoryData/UpdateCategory/{id}`
- **Method**: PUT
- **Description**: Updates a particular category in the system with PUT data input.
- **Parameters**:
  - `id` (int)
  - `category` (Category JSON object)

#### DeleteCategory
- **URL**: `api/CategoryData/DeleteCategory/{id}`
- **Method**: DELETE
- **Description**: Deletes a category from the system by its ID.
- **Parameters**: `id` (int)
- **Returns**: If the category has associated transactions, returns a `409 Conflict` status with a message indicating the category cannot be deleted.

## How to Use

### Prerequisites
- .NET Framework
- Visual Studio or any other IDE that supports ASP.NET

### Setup
1. Clone the repository
   ```bash
   git clone https://github.com/yourusername/PersonalFinanceTracker.git
