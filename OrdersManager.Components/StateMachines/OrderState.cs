using MassTransit;

namespace OrdersManager.Components.StateMachines
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; } = new Guid();
        public string CustomerNumber { get; set; }
        public string CurrentState { get; set; }
        public int Version { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
