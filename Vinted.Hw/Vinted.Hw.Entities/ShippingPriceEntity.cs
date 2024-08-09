using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinted.Hw.Entities
{
    public class ShippingPriceEntity
    {
        public string CarrierCode { get; set; }
        public string PackageSize { get; set; }
        public double Price { get; set; }
    }
}
