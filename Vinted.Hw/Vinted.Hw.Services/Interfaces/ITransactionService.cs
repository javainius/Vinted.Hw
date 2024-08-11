using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Models;

namespace Vinted.Hw.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task<List<TransactionModel>> GetProcessedTransactions(IFormFile transactionLines);
    }
}
