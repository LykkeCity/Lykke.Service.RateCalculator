namespace Lykke.Service.RateCalculator.Core.Domain
{
    public class AssetPairRateModel
    {
        public string QuotingAssetId { get; set; }
        public int Accuracy { get; set; }
        public int InvertedAccuracy { get; set; }
    }
}
