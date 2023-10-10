using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SagaInheritance.App.Sagas;
using SagaInheritance.App.Services;

namespace SagaInheritance.App;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions<MassTransitHostOptions>()
                    .Configure(opt => opt.WaitUntilStarted = true);

                services.AddOptions<LoggerFilterOptions>()
                    .Configure(opt => opt.MinLevel = LogLevel.Information);

                services.AddMassTransit(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();
                    conf.SetSagaRepositoryProvider(new InMemorySagaRepositoryRegistrationProvider());
                        
                    conf.AddConsumersFromNamespaceContaining<Program>();
                    conf.AddSagasFromNamespaceContaining<Program>();
                    conf.AddSagaStateMachinesFromNamespaceContaining<Program>();

                    conf.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddHostedService<MessageSendingService>();
            })
            .ConfigureLogging((hostContext, logging) => logging.AddConsole());

        var host = builder.Build();
        await host.RunAsync();
    }
}