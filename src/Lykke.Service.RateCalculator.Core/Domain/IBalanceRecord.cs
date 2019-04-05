namespace Lykke.Service.RateCalculator.Core.Domain
{
    public interface IBalanceRecord
    {
        string AssetId { get; set; }
        double Balance { get; set; }
    }

    public interface IBalanceRecordWithBase : IBalanceRecord
    {
        string BaseAssetId { get; set; }
        double? AmountInBase { get; set; }
    }

    public class BalanceRecord : IBalanceRecord
    {
        public string AssetId { get; set; }
        public double Balance { get; set; }
    }

    public class BalanceRecordWithBase : IBalanceRecordWithBase
    {
        public string AssetId { get; set; }
        public double Balance { get; set; }
        public string BaseAssetId { get; set; }
        public double? AmountInBase { get; set; }
    }

    public class AssetRequest
    {
        public string AssetId { get; set; }
    }

    public class AssetConversionRate
    {
        public string AssetId { get; set; }

        public double ConversionRate { get; set; }

        public string BaseAssetId { get; set; }
    }
}
