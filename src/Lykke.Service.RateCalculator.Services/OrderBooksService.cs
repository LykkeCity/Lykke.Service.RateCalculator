using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.Assets.Client.ReadModels;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.RateCalculator.Services
{
    public class OrderBookService : IOrderBooksService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IAssetPairsReadModelRepository _assetPairsReadModelRepository;
        private readonly string _orderBooksCacheKeyPattern;

        public OrderBookService(
            IDistributedCache distributedCache,
            IAssetPairsReadModelRepository assetPairsReadModelRepository,
            string orderBooksCacheKeyPattern)
        {
            _distributedCache = distributedCache;
            _assetPairsReadModelRepository = assetPairsReadModelRepository;
            _orderBooksCacheKeyPattern = orderBooksCacheKeyPattern;
        }

        public async Task<IEnumerable<IOrderBook>> GetAllAsync(IEnumerable<string> assetPairIds = null)
        {
            if (assetPairIds == null)
                assetPairIds = _assetPairsReadModelRepository.GetAll().Select(i => i.Id);

            var orderBooks = new List<IOrderBook>();

            foreach (var pairId in assetPairIds)
            {
                var buyBookJson = _distributedCache.GetStringAsync(GetOrderBookKey(pairId, true));
                var sellBookJson = _distributedCache.GetStringAsync(GetOrderBookKey(pairId, false));

                var buyBook = (await buyBookJson)?.DeserializeJson<OrderBook>();

                if (buyBook != null)
                    orderBooks.Add(buyBook);

                var sellBook = (await sellBookJson)?.DeserializeJson<OrderBook>();

                if (sellBook != null)
                    orderBooks.Add(sellBook);
            }

            return orderBooks;
        }

        public async Task<IEnumerable<IOrderBook>> GetAsync(string assetPairId)
        {
            return await GetAllAsync(new List<string> {assetPairId});
        }

        private string GetOrderBookKey(string assetPairId, bool isBuy)
        {
            return string.Format(_orderBooksCacheKeyPattern, assetPairId, isBuy);
        }
    }
}
