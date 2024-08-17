using MassTransit;

namespace OrdersManager.Components.StateMachines
{
    public partial class OrderStateMachine
    {
        public class OrderStatMachineDeffenition : SagaDefinition<OrderState>
        {
            public OrderStatMachineDeffenition()
            {
                ConcurrentMessageLimit = 4;
            }

            protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
            {
                endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
                endpointConfigurator.UseInMemoryOutbox();
            }
        }
    }
}
