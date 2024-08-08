using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinted.Hw.Models
{
    public class FreeShipmentTermModel
    {
        public PackageSize PackageSize { get; set; }
        public string CarrierCode { get; set; }
        public int TimesPerMonth { get; set; }
        public int WhichEveryShipment { get; set; }
    }
}
