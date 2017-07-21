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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MarketProfile
    {
        /// <summary>
        /// Initializes a new instance of the MarketProfile class.
        /// </summary>
        public MarketProfile()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MarketProfile class.
        /// </summary>
        public MarketProfile(IList<IFeedData> profile = default(IList<IFeedData>))
        {
            Profile = profile;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Profile")]
        public IList<IFeedData> Profile { get; set; }

    }
}
