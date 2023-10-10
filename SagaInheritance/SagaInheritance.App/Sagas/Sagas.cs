using MassTransit;
using Microsoft.Extensions.Logging;

namespace SagaInheritance.App.Sagas;

public class MySaga : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string State { get; set; }
    public string Name { get; set; }
}

public class MySagaStateMachine : MassTransitStateMachine<MySaga>
{
    public MySagaStateMachine(ILogger<MySagaStateMachine> logger)
    {
        InstanceState(s => s.State);
        //Event(() => SagaCreated, e => e.CorrelateById(s => s.Message.CorrelationId));
        Initially(
            When(SagaCreated)
                .Then(x => logger.LogInformation("Saga created"))
                .Then(x => x.Saga.Name = x.Message.Name)
        );
    }

    public Event<CreateSaga> SagaCreated { get; }
}

public interface EventBase
{
    Guid CorrelationId { get; }
}

public interface CreateSaga : EventBase
{
    string Name { get; }
}