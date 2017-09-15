using System.Net;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Service.RateCalculator.Extensions
{
    public static class RedisCacheOptionsExtensions
    {
        public static void ResolveDns(this RedisCacheOptions options, string host)
        {
            IPAddress ipAddress;

            if (!IPAddress.TryParse(host, out ipAddress))
            {
                var hostEntry = Dns.GetHostEntryAsync(host).GetAwaiter().GetResult();
                IPAddress ip = null;

                foreach (var address in hostEntry.AddressList)
                {
                    if (IPAddress.TryParse(address.ToString(), out ip))
                        break;
                }

                if (ip != null)
                    options.Configuration = options.Configuration.Replace(host, ip.ToString());
            }
        }
    }
}
