using MassTransit;
using Microsoft.Extensions.Logging;
using OrdersManager.Contracts;
using OrdersManager.SharedModels;

namespace OrdersManager.Components.Consumers
{
    public class OrderPaymentConsumer : IConsumer<OrderPaymentRequest>
    {
        private readonly ILogger<OrderPaymentConsumer> logger;

        public OrderPaymentConsumer(ILogger<OrderPaymentConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPaymentRequest> context)
        {
            var request = context.Message;

            if (request.IsPaid)
            {
                logger.LogInformation($"Order paid: {request.OrderId}");

                await context.Publish<OrderPaid>(new
                {
                    OrderId = request.OrderId,
                    Timestamp = InVar.Timestamp
                });
            }
            else
            {
                logger.LogInformation($"Order cancelled: {request.OrderId}");

                await context.Publish<OrderCancelled>(new
                {
                    OrderId = request.OrderId,
                    Timestamp = InVar.Timestamp
                });
            }
        }
    }

}
