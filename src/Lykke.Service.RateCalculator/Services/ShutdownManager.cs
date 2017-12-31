﻿using Common.Log;
using Lykke.Service.RateCalculator.Core.Services;
using System.Threading.Tasks;

namespace Lykke.Service.RateCalculator.Services
{
    // NOTE: Sometimes, shutdown process should be expressed explicitly. 
    // If this is your case, use this class to manage shutdown.
    // For example, sometimes some state should be saved only after all incoming message processing and 
    // all periodical handler was stopped, and so on.

    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;

        public ShutdownManager(ILog log)
        {
            _log = log;
        }

        public async Task StopAsync()
        {
            // TODO: Implement your shutdown logic here. Good idea is to log every step

            await Task.CompletedTask;
        }
    }
}