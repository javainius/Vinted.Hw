using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Models;

namespace Vinted.Hw.Services.Interfaces
{
    public interface IParsingService
    {
        public List<TransactionModel> ParseTransactions(List<string> transactions);
    }
}
