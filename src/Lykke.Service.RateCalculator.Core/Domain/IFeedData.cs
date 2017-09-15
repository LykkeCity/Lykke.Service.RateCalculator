using System;

namespace Lykke.Service.RateCalculator.Core.Domain
{
    public interface IFeedData
    {
        string Asset { get; }
        DateTime DateTime { get; }
        double Bid { get; }
        double Ask { get; }
    }

    public class FeedData : IFeedData
    {
        public string Asset { get; set; }
        public DateTime DateTime { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }

        public static FeedData Create(IFeedData src)
        {
            return new FeedData
            {
                Asset = src.Asset,
                Ask = src.Ask,
                Bid = src.Bid,
                DateTime = src.DateTime
            };
        }
    }
}
