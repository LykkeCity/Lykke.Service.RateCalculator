using Lykke.Job.MarketProfile.Contract;
using Lykke.Service.RateCalculator.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.RateCalculator.Services
{
    public static class MappingExtensions
    {
        public static FeedData ToApiModel(this IAssetPair src)
        {
            if (src == null) return null;

            return new FeedData
            {
                Ask = src.AskPrice,
                Asset = src.AssetPair,
                Bid = src.BidPrice,
                DateTime = src.AskPriceTimestamp > src.BidPriceTimestamp ? src.AskPriceTimestamp : src.BidPriceTimestamp
            };
        }

        public static Core.Domain.MarketProfile ToApiModel(this IList<IAssetPair> src)
        {
            return new Core.Domain.MarketProfile
            {
                Profile = src.Select(x => x.ToApiModel())
            };
        }
    }
}