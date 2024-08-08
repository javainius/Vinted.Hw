using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinted.Hw.Models
{
    public class TransactionModel
    {
        public string TransactionLine { get; set; }
        public bool IsIgnored { get; set; }
        public TransactionDataModel TransactionData { get; set; }
    }
}
