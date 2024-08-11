using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vinted.Hw.Entities;
using Vinted.Hw.Persistence.Interfaces;

namespace Vinted.Hw.Persistence
{
    public class ShippingPriceRepository : BaseRepository<ShippingPriceEntity>, IShippingPriceRepository
    {
        public ShippingPriceRepository(string filePath) : base(filePath) {}

        public List<ShippingPriceEntity> GetShippingPriceTerms() => GetEntities();
    }
}
