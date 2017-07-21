using System.Threading.Tasks;

namespace Lykke.Service.RateCalculator.Core.Domain
{
    public interface IAssetPairBestPriceRepository
    {
        Task<MarketProfile> GetAsync();
        Task SaveAsync(IFeedData feedData);
    }
}
