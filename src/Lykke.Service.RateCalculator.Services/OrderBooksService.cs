using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.Assets.Client.Models.v3;
using Lykke.Service.Assets.Client.ReadModels;
using Lykke.Service.RateCalculator.Core;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.RateCalculator.Services
{
    public class OrderBookService : IOrderBooksService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IAssetPairsReadModelRepository _assetPairsReadModelRepository;
        private readonly RateCalculatorSettings _settings;

        public OrderBookService(
            IDistributedCache distributedCache,
            RateCalculatorSettings settings,
            IAssetPairsReadModelRepository assetPairsReadModelRepository)
        {
            _distributedCache = distributedCache;
            _settings = settings;
            _assetPairsReadModelRepository = assetPairsReadModelRepository;
        }

        public async Task<IEnumerable<IOrderBook>> GetAllAsync(IEnumerable<AssetPair> assetPairs = null)
        {
            var assetPairIds = assetPairs?.Select(i => i.Id) ?? _assetPairsReadModelRepository.GetAll().Select(i => i.Id);

            var orderBooks = new List<IOrderBook>();

            foreach (var pairId in assetPairIds)
            {
                var buyBookJson = _distributedCache.GetStringAsync(_settings.CacheSettings.GetOrderBookKey(pairId, true));
                var sellBookJson = _distributedCache.GetStringAsync(_settings.CacheSettings.GetOrderBookKey(pairId, false));

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
