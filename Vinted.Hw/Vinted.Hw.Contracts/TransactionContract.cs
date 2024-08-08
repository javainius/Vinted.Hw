using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinted.Hw.Contracts
{
    public class TransactionContract
    {
        public string TransactionLine { get; set; }
        public string IsIgnored { get; set; }
        public TransactionDataContract TransactionData { get; set; }
    }
}
