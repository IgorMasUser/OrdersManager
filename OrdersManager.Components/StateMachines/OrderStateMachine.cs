using MassTransit;
using Microsoft.Extensions.Logging;
using OrdersManager.Contracts;
using OrdersManager.SharedModels;


namespace OrdersManager.Components.Consumers;

public partial class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly ILogger<OrderStateMachine> logger;

    public OrderStateMachine(ILogger<OrderStateMachine> logger)
    {
        this.logger = logger;

        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderPaid, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderCancelled, x => x.CorrelateById(m => m.Message.OrderId));

        InstanceState(x => x.CurrentState);
        Initially(
            When(OrderSubmitted)
            .Then(context =>
            {
                context.Instance.SubmitDate = context.Data.TimeStamp;
                context.Instance.CustomerNumber = context.Data.CustomerNumber;


                logger.Log(LogLevel.Information, $"Order is being submitted. Current State: {context.Instance.CurrentState}");
            })
            .TransitionTo(New));

        During(New,
            When(OrderPaid)
                .Then(context =>
                {
                    context.Instance.UpdatedDate = context.Data.TimeStamp;
                    logger.LogInformation($"Order paid: {context.Instance.CorrelationId}");
                })
                .TransitionTo(Paid),
            When(OrderCancelled)
                .Then(context =>
                {
                    context.Instance.UpdatedDate = context.Data.TimeStamp;
                    logger.LogInformation($"Order cancelled: {context.Instance.CorrelationId}");
                })
                .TransitionTo(Cancelled));    
    }

    // Defining the states
    public State New { get; private set; }
    public State Submitted { get; private set; }
    public State Paid { get; private set; }
    public State Cancelled { get; private set; }

    // Defining events
    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<OrderPaid> OrderPaid { get; private set; }
    public Event<OrderCancelled> OrderCancelled { get; private set; }
}

