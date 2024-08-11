using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Entities;
using Vinted.Hw.Persistence.Interfaces;

namespace Vinted.Hw.Persistence
{
    public class FreeShipmentTermRepository : BaseRepository<FreeShipmentTermEntity>, IFreeShipmentTermRepository
    {
        public FreeShipmentTermRepository(string filePath) : base(filePath) {}

        public FreeShipmentTermEntity GetFreeShipmentTerm() => GetEntity();
    }
}
