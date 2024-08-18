using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace OrdersManager.Components.StateMachines
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        [Key]
        public Guid CorrelationId { get; set; } = new Guid();
        public string CustomerNumber { get; set; }
        public string CurrentState { get; set; }
        public int Version { get; set; }
        public DateTime? SubmitDate { get; internal set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
