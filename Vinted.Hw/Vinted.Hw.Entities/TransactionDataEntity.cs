using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinted.Hw.Entities
{
    public class TransactionDataEntity
    {
        public DateOnly Date { get; set; }
        public string PackageSize { get; set; }
        public string CarrierCode { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
    }
}
