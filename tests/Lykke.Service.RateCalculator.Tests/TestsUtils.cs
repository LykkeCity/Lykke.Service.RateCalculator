using System;
using System.Collections.Generic;
using AzureStorage.Tables;
using Lykke.Service.RateCalculator.AzureRepositories;
using Lykke.Service.RateCalculator.Core.Domain;

namespace Lykke.Service.RateCalculator.Tests
{
    public static class TestsUtils
    {
        public static AssetsRepository GetAssetsRepository()
        {
            var repository = new AssetsRepository(new NoSqlTableInMemory<AssetEntity>());
            var assets = new List<IAsset>
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

            foreach (var asset in assets)
            {
                repository.AddAssetAsync(asset).Wait();
            }

            return repository;
        }

        public static AssetPairsRepository GetAssetPairsRepository()
        {
            var repository = new AssetPairsRepository(new NoSqlTableInMemory<AssetPairEntity>());
            var assetsPairs = new List<IAssetPair>
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

            foreach (var pair in assetsPairs)
            {
                repository.AddAsync(pair).Wait();
            }

            return repository;
        }

        public static AssetPairBestPriceRepository GetBestPriceRepository()
        {
            var repository = new AssetPairBestPriceRepository(new NoSqlTableInMemory<FeedDataEntity>());
            var feed = new List<IFeedData>
            {
                new FeedData {Asset = "BTCUSD", Ask = 2656.381, Bid = 2652, DateTime = DateTime.UtcNow},
                new FeedData {Asset = "USDCHF", Ask = 0.98599, Bid = 0.97925, DateTime = DateTime.UtcNow}
            };

            foreach (var f in feed)
            {
                repository.SaveAsync(f).Wait();
            }

            return repository;
        }
    }
}
