using System.Linq;
using Autofac;
using Common;
using Common.Log;
using Lykke.Service.RateCalculator.AzureRepositories;
using Lykke.Service.RateCalculator.Core;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Lykke.Service.RateCalculator.Extensions;
using Lykke.Service.RateCalculator.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Service.RateCalculator.Modules
{
    public class ServiceModule : Module
    {
        private readonly RateCalculatorSettings _settings;
        private readonly ILog _log;

        public ServiceModule(RateCalculatorSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance<IAssetsRepository>(
                AzureRepoFactories.CreateAssetsRepository(_settings.Db.DictsConnString, _log)
            ).SingleInstance();

            builder.RegisterInstance<IAssetPairsRepository>(
                AzureRepoFactories.CreateAssetPairsRepository(_settings.Db.DictsConnString, _log)
            ).SingleInstance();

            builder.RegisterInstance<IAssetPairBestPriceRepository>(
                AzureRepoFactories.CreateBestPriceRepository(_settings.Db.HLiquidityConnString, _log)
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

            var cacheOptions = new RedisCacheOptions
            {
                Configuration = _settings.CacheSettings.RedisConfiguration,
                InstanceName = _settings.CacheSettings.FinanceDataCacheInstance
            };

            cacheOptions.ResolveDns(_settings.CacheSettings.RedisInternalHost);

            var redis = new RedisCache(cacheOptions);

            builder.RegisterInstance(redis)
                .As<IDistributedCache>()
                .SingleInstance();

            builder.RegisterType<RateCalculatorService>()
                .As<IRateCalculatorService>()
                .SingleInstance();

            builder.RegisterType<OrderBookService>()
                .As<IOrderBooksService>()
                .SingleInstance();
        }
    }
}
