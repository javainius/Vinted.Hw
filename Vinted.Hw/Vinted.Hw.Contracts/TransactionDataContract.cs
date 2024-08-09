using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinted.Hw.Contracts
{
    public class TransactionDataContract
    {
        public DateOnly Date { get; set; }
        public string PackageSize { get; set; }
        public string CarrierCode { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
    }
}
