using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Entities;

namespace Vinted.Hw.Persistence.Interfaces
{
    public interface IFreeShipmentTermRepository
    {
        public FreeShipmentTermEntity GetFreeShipmentTerm();
    }
}
