using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Service.RateCalculator.Core;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.RateCalculator.Services
{
    public class OrderBookService : IOrderBooksService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CachedDataDictionary<string, IAssetPair> _assetPairsDict;
        private readonly RateCalculatorSettings _settings;

        public OrderBookService(
            IDistributedCache distributedCache,
            CachedDataDictionary<string, IAssetPair> assetPairsDict,
            RateCalculatorSettings settings)
        {
            _distributedCache = distributedCache;
            _assetPairsDict = assetPairsDict;
            _settings = settings;
        }

        public async Task<IEnumerable<IOrderBook>> GetAllAsync(IEnumerable<IAssetPair> assetPairs = null)
        {
            var assetPairsToProcess = assetPairs ?? await _assetPairsDict.Values();

            var orderBooks = new List<IOrderBook>();

            foreach (var pair in assetPairsToProcess)
            {
                var buyBookJson = _distributedCache.GetStringAsync(_settings.CacheSettings.GetOrderBookKey(pair.Id, true));
                var sellBookJson = _distributedCache.GetStringAsync(_settings.CacheSettings.GetOrderBookKey(pair.Id, false));

                var buyBook = (await buyBookJson)?.DeserializeJson<OrderBook>();

                if (buyBook != null)
                    orderBooks.Add(buyBook);

                var sellBook = (await sellBookJson)?.DeserializeJson<OrderBook>();

                if (sellBook != null)
                    orderBooks.Add(sellBook);
            }

            return orderBooks;
        }

        public async Task<IEnumerable<IOrderBook>> GetAsync(IAssetPair assetPair)
        {
            return await GetAllAsync(new List<IAssetPair> {assetPair});
        }
    }
}
