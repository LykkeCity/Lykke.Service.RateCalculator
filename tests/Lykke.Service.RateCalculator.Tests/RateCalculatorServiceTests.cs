using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Xunit;

namespace Lykke.Service.RateCalculator.Tests
{
    public class RateCalculatorServiceTests : BaseTests
    {
        private readonly IRateCalculatorService _service;

        public RateCalculatorServiceTests()
        {
            _service = Container.Resolve<IRateCalculatorService>();
        }

        [Fact]
        public void Is_GetRate_Correct()
        {
            var assetPair = new AssetPair
            {
                Id = "BTCUSD",
                BaseAssetId = "BTC",
                QuotingAssetId = "USD",
                Accuracy = 3,
                InvertedAccuracy = 8
            };

            var x = _service.GetRate("BTC", assetPair, 10);
            Assert.Equal(0.1, x);
        }

        [Fact]
        public void Is_GetRate_Inverted_Correct()
        {
            var assetPair = new AssetPair
            {
                Id = "BTCUSD",
                BaseAssetId = "BTC",
                QuotingAssetId = "USD",
                Accuracy = 3,
                InvertedAccuracy = 8
            };

            var x = _service.GetRate("USD", assetPair, 10);
            Assert.Equal(10, x);
        }

        [Fact]
        public async Task Is_FillBaseAssetData_Correct()
        {
            var balances = new List<BalanceRecord>
            {
                new BalanceRecord {AssetId = "CHF", Balance = 10},
                new BalanceRecord {AssetId = "BTC", Balance = 1}
            };
            var balancesWithBaseAsset = (await _service.FillBaseAssetData(balances, "USD")).ToList();

            Assert.Equal(2, balancesWithBaseAsset.Count);
            Assert.Equal(2652, balancesWithBaseAsset.First(item => item.AssetId == "BTC").AmountInBase);
            Assert.Equal(10.14, balancesWithBaseAsset.First(item => item.AssetId == "CHF").AmountInBase);
        }

        [Fact]
        public async Task Is_Single_FillBaseAssetData_Correct()
        {
            var balance = new BalanceRecord {AssetId = "BTC", Balance = 1};

            var balanceWithBaseAsset = await _service.FillBaseAssetData(balance, "USD");

            Assert.NotNull(balanceWithBaseAsset);
            Assert.Equal(2652, balanceWithBaseAsset.AmountInBase);
        }

        [Fact]
        public async Task IS_GetAmountInBase_List_Correct()
        {
            var balances = new List<BalanceRecord>
            {
                new BalanceRecord{AssetId = "CHF", Balance = 10},
                new BalanceRecord{AssetId = "BTC", Balance = 1}
            };

            var balancesWithBaseAsset = (await _service.GetAmountInBase(balances, "USD")).ToList();

            Assert.Equal(2, balancesWithBaseAsset.Count);
            Assert.Equal(10.14, balancesWithBaseAsset[0].Balance);
            Assert.Equal(2652, balancesWithBaseAsset[1].Balance);
        }

        [Fact]
        public async Task Is_GetAmountInBase_Correct()
        {
            var marketProfile = new MarketProfile();
            var feedData = new List<IFeedData>
            {
                new FeedData {Asset = "BTCUSD", Ask = 2656.381, Bid = 2652, DateTime = DateTime.UtcNow}
            };

            marketProfile.Profile = feedData;

            var amountInBase = await _service.GetAmountInBaseWithProfile("BTC", 1, "USD", marketProfile);
            Assert.Equal(2652, amountInBase);
        }

        [Fact]
        public async Task Is_GetAmountInBase_Inverted_Correct()
        {
            var marketProfile = new MarketProfile();
            var feedData = new List<IFeedData>
            {
                new FeedData {Asset = "USDCHF", Ask = 0.98599, Bid = 0.97925, DateTime = DateTime.UtcNow}
            };

            marketProfile.Profile = feedData;

            var amountInBase = await _service.GetAmountInBaseWithProfile("CHF", 1, "USD", marketProfile);
            Assert.Equal(1.01, amountInBase);
        }

        [Fact]
        public void Is_GetMarketAmountInBase_Correct()
        {
            //TODO: add test
        }
    }
}
