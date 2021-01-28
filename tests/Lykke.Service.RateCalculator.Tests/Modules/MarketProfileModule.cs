using Antares.Service.MarketProfile.Client;
using Autofac;
using Lykke.Job.MarketProfile.Contract;
using Moq;
using System;
using System.Collections.Generic;

namespace Lykke.Service.RateCalculator.Tests.Modules
{
    public class MarketProfileModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var marketProfile = new Mock<IMarketProfileServiceClient>();

            marketProfile
                .Setup(x => x.GetAll())
                .Returns(GetFeed());

            builder
                .RegisterInstance(marketProfile.Object)
                .SingleInstance();
        }

        private List<IAssetPair> GetFeed()
        {
            return new List<IAssetPair>
            {
                new FakeAssetPair
                {
                    AssetPair = "BTCUSD",
                    AskPriceTimestamp = DateTime.UtcNow,
                    BidPriceTimestamp = DateTime.Now,
                    BidPrice = 2652,
                    AskPrice = 2656.381
                },
                new FakeAssetPair
                {
                    AssetPair = "USDCHF",
                    AskPriceTimestamp = DateTime.UtcNow,
                    BidPriceTimestamp = DateTime.UtcNow,
                    BidPrice = 0.97925,
                    AskPrice = 0.98599
                }
            };
        }

        public class FakeAssetPair : IAssetPair
        {
            public string AssetPair { get; set; }
            public double BidPrice { get; set; }
            public double AskPrice { get; set; }
            public DateTime BidPriceTimestamp { get; set; }
            public DateTime AskPriceTimestamp { get; set; }
        }
    }
}