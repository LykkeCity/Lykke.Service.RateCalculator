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
        /// <summary>
        /// Adds Rate Calculator client to the ContainerBuilder.
        /// </summary>
        /// <param name="builder">ContainerBuilder instance.</param>
        /// <param name="serviceUrl">Effective Rate Calculator service location.</param>
        /// <param name="log">Logger.</param>
        [Obsolete("Please, use the overload without explicitly passed logger.")]
        public static void RegisterRateCalculatorClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterInstance(new RateCalculatorClient(serviceUrl, log)).As<IRateCalculatorClient>().SingleInstance();
        }

        /// <summary>
        /// Adds Rate Calculator client to the ContainerBuilder. The implementation of ILogFactory should be already injected.
        /// </summary>
        /// <param name="builder">ContainerBuilder instance.</param>
        /// <param name="serviceUrl">Effective Rate Calculator service location.</param>
        public static void RegisterRateCalculatorClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.Register(ctx => new RateCalculatorClient(
                serviceUrl, 
                ctx.Resolve<ILogFactory>()))
                .As<IRateCalculatorClient>()
                .SingleInstance();
        }
    }
}
