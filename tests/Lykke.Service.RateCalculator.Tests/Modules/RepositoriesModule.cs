using Autofac;
using Lykke.Service.RateCalculator.Core.Domain;

namespace Lykke.Service.RateCalculator.Tests.Modules
{
    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IAssetPairBestPriceRepository>(
                TestsUtils.GetBestPriceRepository()
            ).SingleInstance();

            builder.RegisterInstance<IAssetsRepository>(
                TestsUtils.GetAssetsRepository()
            ).SingleInstance();

            builder.RegisterInstance<IAssetPairsRepository>(
                TestsUtils.GetAssetPairsRepository()
            ).SingleInstance();
        }
    }
}
