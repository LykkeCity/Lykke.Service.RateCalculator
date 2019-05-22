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

    public partial class FeedData
    {
        /// <summary>
        /// Initializes a new instance of the FeedData class.
        /// </summary>
        public FeedData()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the FeedData class.
        /// </summary>
        public FeedData(System.DateTime dateTime, double bid, double ask, string asset = default(string))
        {
            Asset = asset;
            DateTime = dateTime;
            Bid = bid;
            Ask = ask;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Asset")]
        public string Asset { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DateTime")]
        public System.DateTime DateTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Bid")]
        public double Bid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Ask")]
        public double Ask { get; set; }

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
