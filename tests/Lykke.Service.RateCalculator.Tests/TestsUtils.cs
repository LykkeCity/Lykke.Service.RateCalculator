using System.Collections.Generic;
using Lykke.Service.Assets.Client.Models.v3;

namespace Lykke.Service.RateCalculator.Tests
{
    public static class TestsUtils
    {
        public static List<Asset> GetAssetsRepository()
        {
            var assets = new List<Asset>
            {
                new Asset
                {
                    Accuracy = 8,
                    Id = "BTC"
                },
                new Asset
                {
                    Accuracy = 2,
                    Id = "USD"
                },
                new Asset
                {
                    Accuracy = 2,
                    Id = "CHF"
                }
            };

            return assets;
        }

        public static List<AssetPair> GetAssetPairsRepository()
        {
            var assetsPairs = new List<AssetPair>
            {
                new AssetPair
                {
                    Id = "BTCCHF",
                    Accuracy = 3,
                    InvertedAccuracy = 8,
                    BaseAssetId = "BTC",
                    QuotingAssetId = "CHF"
                },
                new AssetPair
                {
                    Id = "BTCUSD",
                    Accuracy = 3,
                    InvertedAccuracy = 8,
                    BaseAssetId = "BTC",
                    QuotingAssetId = "USD"
                },
                new AssetPair
                {
                    Id = "USDCHF",
                    Accuracy = 2,
                    InvertedAccuracy = 2,
                    BaseAssetId = "USD",
                    QuotingAssetId = "CHF"
                }
            };

            return assetsPairs;
        }
    }
}
