using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Cache;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.RateCalculator.Core.Domain;

namespace Lykke.Service.RateCalculator.Tests.Modules
{
    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IAssetsServiceWithCache>(new MockedAssetsServiceWithCache(TestsUtils.GetAssetsRepository(), TestsUtils.GetAssetPairsRepository()));
        }
    }

    public class MockedAssetsServiceWithCache : IAssetsServiceWithCache
    {
        private readonly List<Asset> _assets;
        private readonly List<AssetPair> _assetPairs;

        public MockedAssetsServiceWithCache(List<Asset> assets, List<AssetPair> assetPairs)
        {
            _assets = assets;
            _assetPairs = assetPairs;
        }

        public Task<IReadOnlyCollection<AssetPair>> GetAllAssetPairsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            IReadOnlyCollection<AssetPair> result = _assetPairs;
            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<Asset>> GetAllAssetsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            IReadOnlyCollection<Asset> result = _assets;
            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<Asset>> GetAllAssetsAsync(bool includeNonTradable, CancellationToken cancellationToken = new CancellationToken())
        {
            IReadOnlyCollection<Asset> result = _assets;
            return Task.FromResult(result);
        }

        public Task<Asset> TryGetAssetAsync(string assetId, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(_assets.FirstOrDefault(x => x.Id == assetId));
        }

        public Task<AssetPair> TryGetAssetPairAsync(string assetPairId, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(_assetPairs.FirstOrDefault(x => x.Id == assetPairId));
        }

        public Task UpdateAssetPairsCacheAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAssetsCacheAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}
