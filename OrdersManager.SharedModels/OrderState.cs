using MassTransit;

namespace OrdersManager.SharedModels
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CurrentState { get; set; }
        public int Version { get; set; }
        public DateTime? SubmitDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        public Order? Order { get; set; }
    }
}
