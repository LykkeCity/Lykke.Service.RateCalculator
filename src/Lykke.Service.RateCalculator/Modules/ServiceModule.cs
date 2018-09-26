using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.MarketProfile.Client;
using Lykke.Service.RateCalculator.Core;
using Lykke.Service.RateCalculator.Core.Services;
using Lykke.Service.RateCalculator.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.RateCalculator.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;
        private readonly ServiceCollection _serviceCollection;

        public ServiceModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
            _serviceCollection = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings.CurrentValue.RateCalculatorService)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterAssetsClient(_settings.CurrentValue.AssetsServiceClient.ServiceUrl);

            var financeRedisCache = new RedisCache(new RedisCacheOptions
            {
                Configuration = _settings.CurrentValue.RateCalculatorService.CacheSettings.RedisConfiguration,
                InstanceName = _settings.CurrentValue.RateCalculatorService.CacheSettings.FinanceDataCacheInstance
            });

            builder.RegisterInstance(financeRedisCache)
                .As<IDistributedCache>()
                .SingleInstance();

            builder.RegisterType<RateCalculatorService>()
                .As<IRateCalculatorService>()
                .SingleInstance();

            builder.RegisterType<OrderBookService>()
                .As<IOrderBooksService>()
                .SingleInstance();

            builder.RegisterType<LykkeMarketProfile>()
                .As<ILykkeMarketProfile>()
                .WithParameter("baseUri", new Uri(_settings.CurrentValue.MarketProfileServiceClient.ServiceUrl));

            builder.Populate(_serviceCollection);
        }
    }
}
