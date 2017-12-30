using System;
using System.Linq;
using Autofac;
using Common;
using Common.Log;
using Lykke.Service.MarketProfile.Client;
using Lykke.Service.RateCalculator.AzureRepositories;
using Lykke.Service.RateCalculator.Core;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Lykke.Service.RateCalculator.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Service.RateCalculator.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
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

            builder.RegisterInstance<IAssetsRepository>(
                AzureRepoFactories.CreateAssetsRepository(_settings.ConnectionString(x => x.RateCalculatorService.Db.DictsConnString), _log)
            ).SingleInstance();

            builder.RegisterInstance<IAssetPairsRepository>(
                AzureRepoFactories.CreateAssetPairsRepository(_settings.ConnectionString(x => x.RateCalculatorService.Db.DictsConnString), _log)
            ).SingleInstance();

            builder.RegisterInstance<IAssetPairBestPriceRepository>(
                AzureRepoFactories.CreateBestPriceRepository(_settings.ConnectionString(x => x.RateCalculatorService.Db.HLiquidityConnString), _log)
            ).SingleInstance();

            builder.Register(x =>
            {
                var ctx = x.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, IAsset>(async () => (await ctx.Resolve<IAssetsRepository>().GetAssetsAsync()).ToDictionary(itm => itm.Id));
            }).SingleInstance();

            builder.Register(x =>
            {
                var ctx = x.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, IAssetPair>(
                    async () => (await ctx.Resolve<IAssetPairsRepository>().GetAllAsync()).ToDictionary(itm => itm.Id));
            }).SingleInstance();

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
        }
    }
}
