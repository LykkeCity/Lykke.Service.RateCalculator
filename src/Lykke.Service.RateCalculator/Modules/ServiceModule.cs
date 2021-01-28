using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.RateCalculator.Core;
using Lykke.Service.RateCalculator.Core.Services;
using Lykke.Service.RateCalculator.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;
using Antares.Service.MarketProfile.Client;

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
            var appSettings = _settings.CurrentValue;

            builder.RegisterInstance(appSettings.RateCalculatorService)
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

            builder.RegisterAssetsClient(appSettings.AssetsServiceClient.ServiceUrl);

            var financeRedisCache = new RedisCache(new RedisCacheOptions
            {
                Configuration = appSettings.RateCalculatorService.CacheSettings.RedisConfiguration,
                InstanceName = appSettings.RateCalculatorService.CacheSettings.FinanceDataCacheInstance
            });

            builder.RegisterInstance(financeRedisCache)
                .As<IDistributedCache>()
                .SingleInstance();

            builder.RegisterType<RateCalculatorService>()
                .As<IRateCalculatorService>()
                .SingleInstance();

            builder.RegisterType<OrderBookService>()
                .As<IOrderBooksService>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(appSettings.RateCalculatorService.CacheSettings.OrderBooksCacheKeyPattern));

            builder.Register((x) =>
                {
                    var marketProfile = new MarketProfileServiceClient(_settings.CurrentValue.MyNoSqlServer.ReaderUrl, _settings.CurrentValue.MarketProfileServiceClient.ServiceUrl);
                    marketProfile.Start();

                    return marketProfile;
                })
                .As<IMarketProfileServiceClient>()
                .SingleInstance()
                .AutoActivate();

            builder.Populate(_serviceCollection);
        }
    }
}
