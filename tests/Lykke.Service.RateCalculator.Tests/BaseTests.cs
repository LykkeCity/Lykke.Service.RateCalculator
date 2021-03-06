﻿using Autofac;
using Common.Log;
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

            builder.RegisterInstance(new EmptyLog()).As<ILog>().SingleInstance();

            builder.RegisterModule(new RepositoriesModule());

            builder.RegisterInstance(
                new Mock<IOrderBooksService>().Object
            ).SingleInstance();

            builder.RegisterType<RateCalculatorService>()
                .As<IRateCalculatorService>()
                .SingleInstance();

            builder.RegisterModule(new MarketProfileModule());

            Container = builder.Build();
        }
    }
}
