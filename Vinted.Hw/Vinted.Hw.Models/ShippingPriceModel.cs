using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinted.Hw.Models
{
    public class ShippingPriceModel
    {
        public string CarrierCode { get; set; }
        public PackageSize PackageSize { get; set; }
        public double Price { get; set; }
    }
}
