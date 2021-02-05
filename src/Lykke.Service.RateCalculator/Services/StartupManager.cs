using Common.Log;
using Lykke.Service.RateCalculator.Core.Services;
using System.Threading.Tasks;
using Antares.Service.MarketProfile.Client;

namespace Lykke.Service.RateCalculator.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IMarketProfileServiceClient _marketProfileServiceClient;

        public StartupManager(ILog log,
            IMarketProfileServiceClient marketProfileServiceClient)
        {
            _log = log;
            _marketProfileServiceClient = marketProfileServiceClient;
        }

        public async Task StartAsync()
        {
            _marketProfileServiceClient.Start();

            await Task.CompletedTask;
        }
    }
}