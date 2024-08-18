﻿using MassTransit;
using OrdersManager.Contracts;

namespace OrdersManager.Components.StateMachines
{
    public partial class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                //x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                //  {
                //      if (context.RequestId.HasValue)
                //      {
                //          await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
                //      }

                //  }));
            });

            InstanceState(x => x.CurrentState);
            Initially(
                When(OrderSubmitted)
                .Then(context =>
                {
                    context.Instance.SubmitDate = context.Data.TimeStamp;
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                })
                .TransitionTo(Submitted));

            During(Submitted, Ignore(OrderSubmitted));

            DuringAny(When(OrderStatusRequested)
                .RespondAsync(x => x.Init<OrderStatus>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState
                }))
            );

            DuringAny(
                When(OrderSubmitted)
                .Then(context =>
                {
                    context.Instance.SubmitDate ??= context.Data.TimeStamp;
                    context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
                }));
        }

        public State Submitted { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }
    }
}
