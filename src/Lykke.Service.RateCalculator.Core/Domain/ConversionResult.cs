namespace Lykke.Service.RateCalculator.Core.Domain
{
    public class ConversionResult
    {
        public AssetWithAmount From { get; set; }
        public AssetWithAmount To { get; set; }
        public double Price { get; set; }

        /// <summary>
        /// Price from order book according to volume
        /// </summary>
        public double VolumePrice { get; set; }

        public OperationResult Result { get; set; }
    }
}
