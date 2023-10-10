using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SagaInheritance.App.Sagas;

namespace SagaInheritance.App.Services;

public class MessageSendingService : BackgroundService
{
    private readonly IBus _bus;
    private readonly ILogger<MessageSendingService> _logger;

    public MessageSendingService(IBus bus, ILogger<MessageSendingService> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sending test message");
        
        await _bus.Publish<CreateSaga>(new
        {
            CorrelationId = NewId.Next(),
            Name = "Test Saga"
        }, stoppingToken);
        
        _logger.LogInformation("Sent");
    }
}