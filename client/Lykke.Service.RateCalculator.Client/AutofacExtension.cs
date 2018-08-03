using System;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;

namespace Lykke.Service.RateCalculator.Client
{
    [PublicAPI]
    public static class AutofacExtension
    {
        [Obsolete("Please, use the overload which does not explicitly require ILog.")]
        public static void RegisterRateCalculatorClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterInstance(new RateCalculatorClient(serviceUrl, log)).As<IRateCalculatorClient>().SingleInstance();
        }

        public static void RegisterRateCalculatorClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.Register(s => new RateCalculatorClient(serviceUrl, s.Resolve<ILogFactory>()))
                .As<IRateCalculatorClient>()
                .SingleInstance();
        }
    }
}
