using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client.Models.v3;
using Lykke.Service.RateCalculator.Core.Domain;

namespace Lykke.Service.RateCalculator.Core.Services
{
    public interface IOrderBooksService
    {
        Task<IEnumerable<IOrderBook>> GetAllAsync(IEnumerable<AssetPair> assetPairs = null);
        Task<IEnumerable<IOrderBook>> GetAsync(AssetPair assetPair);
    }
}
