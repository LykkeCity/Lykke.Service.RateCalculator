using Autofac;
using Lykke.Service.MarketProfile.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Lykke.Service.MarketProfile.Client.Models;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace Lykke.Service.RateCalculator.Tests.Modules
{
    public class MarketProfileModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var mpMock = new Mock<ILykkeMarketProfile>();

            var mockedHttpResponse = new HttpOperationResponse<IList<AssetPairModel>> {Body = GetFeed()};
            mpMock
                .Setup(x => x.ApiMarketProfileGetWithHttpMessagesAsync(
                    It.IsAny<Dictionary<string, List<string>>>(), 
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mockedHttpResponse));

            builder
                .RegisterInstance(mpMock.Object)
                .SingleInstance();
        }

        private IList<AssetPairModel> GetFeed()
        {
            return new List<AssetPairModel>
            {
                new AssetPairModel
                {
                    AssetPair = "BTCUSD",
                    AskPriceTimestamp = DateTime.UtcNow,
                    BidPriceTimestamp = DateTime.Now,
                    BidPrice = 2652,
                    AskPrice = 2656.381
                },
                new AssetPairModel
                {
                    AssetPair = "USDCHF",
                    AskPriceTimestamp = DateTime.UtcNow,
                    BidPriceTimestamp = DateTime.UtcNow,
                    BidPrice = 0.97925,
                    AskPrice = 0.98599
                }
            };
        }
    }
}