using System.Collections.Generic;
using Lykke.Service.Assets.Client.Models.v3;

namespace Lykke.Service.RateCalculator.Tests
{
    internal static class TestsUtils
    {
        internal static int FiatAccuracy = 2;
        internal static int CryptoAccuracy = 8;

        internal static List<Asset> GetAssetsRepository()
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
                    Accuracy = FiatAccuracy,
                    Id = "USD"
                },
                new Asset
                {
                    Accuracy = FiatAccuracy,
                    Id = "CHF"
                },
                new Asset
                {
                    Accuracy = CryptoAccuracy,
                    Id = "SLR"
                }
            };

            return assets;
        }

        internal static List<AssetPair> GetAssetPairsRepository()
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
                },
                new AssetPair
                {
                    Id = "SLRUSD",
                    Accuracy = 8,
                    InvertedAccuracy = 5,
                    BaseAssetId = "SLR",
                    QuotingAssetId = "USD"
                }
            };

            return assetsPairs;
        }
    }
}
