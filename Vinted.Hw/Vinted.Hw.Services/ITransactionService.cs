using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Models;

namespace Vinted.Hw.Services
{
    public interface ITransactionService
    {
        public List<TransactionModel> GetProcessedTransactions(List<string> transactionLines);
    }
}
