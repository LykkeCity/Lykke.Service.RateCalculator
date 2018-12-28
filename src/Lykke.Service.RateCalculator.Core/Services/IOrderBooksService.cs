﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.RateCalculator.Core.Domain;

namespace Lykke.Service.RateCalculator.Core.Services
{
    public interface IOrderBooksService
    {
        Task<IEnumerable<IOrderBook>> GetAllAsync(IEnumerable<string> assetPairIds = null);
        Task<IEnumerable<IOrderBook>> GetAsync(string assetPairId);
    }
}
