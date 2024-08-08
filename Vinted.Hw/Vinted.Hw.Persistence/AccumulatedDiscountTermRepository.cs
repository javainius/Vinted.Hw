using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Entities;

namespace Vinted.Hw.Persistence
{
    public class AccumulatedDiscountTermRepository : BaseRepository<AccumulatedDiscountTermEntity>, IAccumulatedDiscountTermRepository
    {
        public AccumulatedDiscountTermRepository(string filePath) : base(filePath) {}

        public AccumulatedDiscountTermEntity GetAccumulatedDiscountTerm() => GetEntities().First();
    }
}
