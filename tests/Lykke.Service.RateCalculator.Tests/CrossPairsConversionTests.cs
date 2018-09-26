using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Xunit;

namespace Lykke.Service.RateCalculator.Tests
{
    public class CrossPairsConversionTests : BaseTests
    {
        private readonly IRateCalculatorService _service;

        public CrossPairsConversionTests()
        {
            _service = Container.Resolve<IRateCalculatorService>();
        }

        [Fact]
        public async Task StraightTrueTrueConversion()
        {
            var marketProfile = new Core.Domain.MarketProfile();
            var btcBid = 8600;
            var chfBid = 0.97925;
            var feedData = new List<FeedData>
            {
                new FeedData {Asset = "BTCUSD", Ask = 9000, Bid = btcBid, DateTime = DateTime.UtcNow},
                new FeedData {Asset = "USDCHF", Ask = 0.98599, Bid = chfBid, DateTime = DateTime.UtcNow},
            };

            marketProfile.Profile = feedData;

            var amountInBase = await _service.GetAmountInBaseWithProfile("BTC", 1, "CHF", marketProfile);
            Assert.Equal((btcBid * chfBid).TruncateDecimalPlaces(TestsUtils.FiatAccuracy), amountInBase);
        }

        [Fact]
        public async Task StraightTrueFalseConversion()
        {
            var marketProfile = new Core.Domain.MarketProfile();
            var btcBid = 8600;
            var slrAsk = 0.4508;
            var feedData = new List<FeedData>
            {
                new FeedData {Asset = "BTCUSD", Ask = 9000, Bid = btcBid, DateTime = DateTime.UtcNow},
                new FeedData {Asset = "SLRUSD", Ask = slrAsk, Bid = 0.0003, DateTime = DateTime.UtcNow},
            };

            marketProfile.Profile = feedData;

            var amountInBase = await _service.GetAmountInBaseWithProfile("BTC", 1, "SLR", marketProfile);
            Assert.Equal((btcBid / slrAsk).TruncateDecimalPlaces(TestsUtils.CryptoAccuracy), amountInBase);
        }

        [Fact]
        public async Task StraightFalseTrueConversion()
        {
            var marketProfile = new Core.Domain.MarketProfile();
            var btcBid = 8600;
            var chfAsk = 0.98599;
            var feedData = new List<FeedData>
            {
                new FeedData {Asset = "BTCCHF", Ask = chfAsk, Bid = 0.97925, DateTime = DateTime.UtcNow},
                new FeedData {Asset = "BTCUSD", Ask = 9000, Bid = btcBid, DateTime = DateTime.UtcNow},
            };

            marketProfile.Profile = feedData;

            var amountInBase = await _service.GetAmountInBaseWithProfile("CHF", 1, "USD", marketProfile);
            Assert.Equal((btcBid / chfAsk).TruncateDecimalPlaces(TestsUtils.FiatAccuracy), amountInBase);
        }

        [Fact]
        public async Task StraightFalseFalseConversion()
        {
            var marketProfile = new Core.Domain.MarketProfile();
            var btcAsk = 9000;
            var chfAsk = 0.98599;
            var feedData = new List<FeedData>
            {
                new FeedData {Asset = "BTCUSD", Ask = btcAsk, Bid = 8600, DateTime = DateTime.UtcNow},
                new FeedData {Asset = "USDCHF", Ask = chfAsk, Bid = 0.97925, DateTime = DateTime.UtcNow},
            };

            marketProfile.Profile = feedData;

            var amountInBase = await _service.GetAmountInBaseWithProfile("CHF", 1, "BTC", marketProfile);
            Assert.Equal((1 / chfAsk / btcAsk).TruncateDecimalPlaces(TestsUtils.CryptoAccuracy), amountInBase);
        }
    }
}
