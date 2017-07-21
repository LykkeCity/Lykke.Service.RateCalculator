using System.Linq;
using Autofac;
using Common;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Lykke.Service.RateCalculator.Services;
using Lykke.Service.RateCalculator.Tests.Modules;
using Moq;

namespace Lykke.Service.RateCalculator.Tests
{
    public class BaseTests
    {
        protected IContainer Container { get; set; }

        public BaseTests()
        {
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new RepositoriesModule());

            builder.Register(x =>
            {
                var ctx = x.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, IAsset>(async () => (await ctx.Resolve<IAssetsRepository>().GetAssetsAsync()).ToDictionary(itm => itm.Id));
            }).SingleInstance();

            builder.Register(x =>
            {
                var ctx = x.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, IAssetPair>(async () => (await ctx.Resolve<IAssetPairsRepository>().GetAllAsync()).ToDictionary(itm => itm.Id));
            }).SingleInstance();

            builder.RegisterInstance(
                new Mock<IOrderBooksService>().Object
            ).SingleInstance();

            builder.RegisterType<RateCalculatorService>()
                .As<IRateCalculatorService>()
                .SingleInstance();

            Container = builder.Build();
        }
    }
}
