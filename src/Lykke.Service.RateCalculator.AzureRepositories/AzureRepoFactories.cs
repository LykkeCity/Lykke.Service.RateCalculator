using AzureStorage.Tables;
using Common.Log;

namespace Lykke.Service.RateCalculator.AzureRepositories
{
    public static class AzureRepoFactories
    {
        private const string TableNameDictionaries = "Dictionaries";

        public static AssetsRepository CreateAssetsRepository(string connstring, ILog log)
        {
            return new AssetsRepository(new AzureTableStorage<AssetEntity>(connstring, TableNameDictionaries, log));
        }

        public static AssetPairsRepository CreateAssetPairsRepository(string connString, ILog log)
        {
            return new AssetPairsRepository(new AzureTableStorage<AssetPairEntity>(connString, TableNameDictionaries, log));
        }

        public static AssetPairBestPriceRepository CreateBestPriceRepository(string connString, ILog log)
        {
            return new AssetPairBestPriceRepository(new AzureTableStorage<FeedDataEntity>(connString, "MarketProfile", log));
        }
    }
}
