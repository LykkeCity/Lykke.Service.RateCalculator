// Code generated by Microsoft (R) AutoRest Code Generator 1.1.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.Service.RateCalculator.Client.AutorestClient.Models
{
    using Lykke.Service;
    using Lykke.Service.RateCalculator;
    using Lykke.Service.RateCalculator.Client;
    using Lykke.Service.RateCalculator.Client.AutorestClient;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class ConversionResult
    {
        /// <summary>
        /// Initializes a new instance of the ConversionResult class.
        /// </summary>
        public ConversionResult()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ConversionResult class.
        /// </summary>
        /// <param name="result">Possible values include: 'Unknown', 'Ok',
        /// 'InvalidInputParameters', 'NoLiquidity'</param>
        public ConversionResult(double price, double volumePrice, OperationResult result, AssetWithAmount fromProperty = default(AssetWithAmount), AssetWithAmount to = default(AssetWithAmount))
        {
            FromProperty = fromProperty;
            To = to;
            Price = price;
            VolumePrice = volumePrice;
            Result = result;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "From")]
        public AssetWithAmount FromProperty { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "To")]
        public AssetWithAmount To { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Price")]
        public double Price { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "VolumePrice")]
        public double VolumePrice { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Unknown', 'Ok',
        /// 'InvalidInputParameters', 'NoLiquidity'
        /// </summary>
        [JsonProperty(PropertyName = "Result")]
        public OperationResult Result { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (FromProperty != null)
            {
                FromProperty.Validate();
            }
            if (To != null)
            {
                To.Validate();
            }
        }
    }
}
