using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.RateCalculator.Core.Domain
{
    public class MarketProfile
    {
        public IEnumerable<IFeedData> Profile { get; set; }
    }

    public static class MarketProfileExt
    {
        public static double? GetAsk(this MarketProfile marketProfile, string assetPairId)
        {
            return marketProfile.Profile?.FirstOrDefault(x => x.Asset == assetPairId)?.Ask;
        }

        public static double? GetBid(this MarketProfile marketProfile, string assetPairId)
        {
            return marketProfile.Profile?.FirstOrDefault(x => x.Asset == assetPairId)?.Bid;
        }

        public static double? GetPrice(this MarketProfile marketProfile, string assetPairId, OrderAction orderAction)
        {
            return orderAction == OrderAction.Sell ? GetAsk(marketProfile, assetPairId) : GetBid(marketProfile, assetPairId);
        }
    }
}
