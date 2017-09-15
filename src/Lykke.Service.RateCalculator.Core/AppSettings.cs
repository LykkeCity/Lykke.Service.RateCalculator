namespace Lykke.Service.RateCalculator.Core
{
    public class AppSettings
    {
        public RateCalculatorSettings RateCalculatorService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }

    public class RateCalculatorSettings
    {
        public DbSettings Db { get; set; }
        public CacheSettings CacheSettings { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string HLiquidityConnString { get; set; }
        public string DictsConnString { get; set; }
    }

    public class CacheSettings
    {
        public string FinanceDataCacheInstance { get; set; }
        public string RedisConfiguration { get; set; }
        public int RedisPort { get; set; }
        public string RedisInternalHost { get; set; }

        public string OrderBooksCacheKeyPattern { get; set; }
        public string AssetsForClientCacheKeyPattern { get; set; }
    }

    public static class CacheSettingsExt
    {
        public static string GetOrderBookKey(this CacheSettings settings, string assetPairId, bool isBuy)
        {
            return string.Format(settings.OrderBooksCacheKeyPattern, assetPairId, isBuy);
        }

        public static string GetAssetsForClientKey(this CacheSettings settings, string clientId, bool isIosDevice)
        {
            return string.Format(settings.AssetsForClientCacheKeyPattern, clientId, isIosDevice);
        }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }

        public int ThrottlingLimitSeconds { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
