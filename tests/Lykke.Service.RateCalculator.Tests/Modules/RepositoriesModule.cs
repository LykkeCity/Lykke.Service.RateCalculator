using System.Collections.Generic;
using System.Linq;
using Autofac;
using Lykke.Service.Assets.Client.Models.v3;
using Lykke.Service.Assets.Client.ReadModels;

namespace Lykke.Service.RateCalculator.Tests.Modules
{
    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IAssetsReadModelRepository>(new MockedAssetsRepository(TestsUtils.GetAssetsRepository()));
            builder.RegisterInstance<IAssetPairsReadModelRepository>(new MockedAssetPairsRepository(TestsUtils.GetAssetPairsRepository()));
        }
    }

    public class MockedAssetsRepository : IAssetsReadModelRepository
    {
        private readonly List<Asset> _assets;

        public MockedAssetsRepository(List<Asset> assets)
        {
            _assets = assets;
        }

        public Asset TryGet(string id)
        {
            return _assets.FirstOrDefault(x => x.Id == id);
        }

        public IReadOnlyCollection<Asset> GetAll()
        {
            IReadOnlyCollection<Asset> result = _assets;
            return result;
        }

        public void Add(Asset assetPair)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Asset assetPair)
        {
            throw new System.NotImplementedException();
        }
    }

    public class MockedAssetPairsRepository : IAssetPairsReadModelRepository
    {
        private readonly List<AssetPair> _assetPairs;

        public MockedAssetPairsRepository(List<AssetPair> assetPairs)
        {
            _assetPairs = assetPairs;
        }

        public AssetPair TryGet(string id)
        {
            return _assetPairs.FirstOrDefault(x => x.Id == id);
        }

        public IReadOnlyCollection<AssetPair> GetAll()
        {
            IReadOnlyCollection<AssetPair> result = _assetPairs;
            return result;
        }

        public void Add(AssetPair assetPair)
        {
            throw new System.NotImplementedException();
        }

        public void Update(AssetPair assetPair)
        {
            throw new System.NotImplementedException();
        }
    }
}
