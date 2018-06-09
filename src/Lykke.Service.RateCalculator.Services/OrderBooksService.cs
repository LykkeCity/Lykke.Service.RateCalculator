using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.RateCalculator.Core;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.RateCalculator.Services
{
    public class OrderBookService : IOrderBooksService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly RateCalculatorSettings _settings;

        public OrderBookService(
            IDistributedCache distributedCache,
            RateCalculatorSettings settings, 
            IAssetsServiceWithCache assetsServiceWithCache)
        {
            _distributedCache = distributedCache;
            _settings = settings;
            _assetsServiceWithCache = assetsServiceWithCache;
        }

        public async Task<IEnumerable<IOrderBook>> GetAllAsync(IEnumerable<AssetPair> assetPairs = null)
        {
            var assetPairsToProcess = assetPairs ?? await _assetsServiceWithCache.GetAllAssetPairsAsync();

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

        public async Task<IEnumerable<IOrderBook>> GetAsync(AssetPair assetPair)
        {
            return await GetAllAsync(new List<AssetPair> {assetPair});
        }
    }
}
