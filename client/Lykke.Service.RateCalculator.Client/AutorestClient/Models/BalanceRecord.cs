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

    public partial class BalanceRecord
    {
        /// <summary>
        /// Initializes a new instance of the BalanceRecord class.
        /// </summary>
        public BalanceRecord()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BalanceRecord class.
        /// </summary>
        public BalanceRecord(double balance, string assetId = default(string))
        {
            AssetId = assetId;
            Balance = balance;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AssetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Balance")]
        public double Balance { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
