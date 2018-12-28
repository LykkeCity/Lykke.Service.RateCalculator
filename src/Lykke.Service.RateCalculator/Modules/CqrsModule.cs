using System.Collections.Generic;
using Autofac;
using Common.Log;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using Lykke.Service.Assets.Client;
using Lykke.SettingsReader;

namespace Lykke.Service.RateCalculator.Modules
{
    public class CqrsModule : Module
    {
        private readonly CqrsSettings _cqrsSettings;
        private readonly ILog _log;

        public CqrsModule(IReloadingManager<AppSettings> settingsManager, ILog log)
        {
            _cqrsSettings = settingsManager.CurrentValue.RateCalculatorService.CqrsSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(ctx =>
                {
                    var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory { Uri = _cqrsSettings.RabbitConnectionString };
                    var me = new MessagingEngine(
                        _log,
                        new TransportResolver(new Dictionary<string, TransportInfo>
                        {
                            {
                                "RabbitMq",
                                new TransportInfo(
                                    rabbitMqSettings.Endpoint.ToString(),
                                    rabbitMqSettings.UserName,
                                    rabbitMqSettings.Password,
                                    "None",
                                    "RabbitMq")
                            }
                        }),
                        new RabbitMqTransportFactory());
                    var engine = new CqrsEngine(
                        _log,
                        new AutofacDependencyResolver(ctx),
                        me,
                        new DefaultEndpointProvider(),
                        true,
                        Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                            "RabbitMq",
                            SerializationFormat.MessagePack,
                            environment: "lykke")),
                        Register.BoundedContext("rate-calculator")
                            .WithAssetsReadModel(AppEnvironment.EnvInfo)
                    );
                    engine.StartSubscribers();
                    return engine;
                })
                .As<ICqrsEngine>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
