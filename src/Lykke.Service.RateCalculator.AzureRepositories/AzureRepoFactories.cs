using AzureStorage.Tables;
using Common.Log;
using Lykke.SettingsReader;

namespace Lykke.Service.RateCalculator.AzureRepositories
{
    public static class AzureRepoFactories
    {
        private const string TableNameDictionaries = "Dictionaries";

        public static AssetsRepository CreateAssetsRepository(IReloadingManager<string> connString, ILog log)
        {
            return new AssetsRepository(AzureTableStorage<AssetEntity>.Create(connString, TableNameDictionaries, log));
        }

        public static AssetPairsRepository CreateAssetPairsRepository(IReloadingManager<string> connString, ILog log)
        {
            return new AssetPairsRepository(AzureTableStorage<AssetPairEntity>.Create(connString, TableNameDictionaries, log));
        }

        public static AssetPairBestPriceRepository CreateBestPriceRepository(IReloadingManager<string> connString, ILog log)
        {
            return new AssetPairBestPriceRepository(AzureTableStorage<FeedDataEntity>.Create(connString, "MarketProfile", log));
        }
    }
}
