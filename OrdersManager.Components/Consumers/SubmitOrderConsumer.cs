using MassTransit;
using Microsoft.Extensions.Logging;
using OrdersManager.Contracts;

namespace OrdersManager.Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            logger.Log(LogLevel.Debug, "SubmitOrderConsumer:{CustomerNumber}", context.Message.CustomerNumber);
            //if (context.Message.CustomerNumber.Contains("TEST"))
            //{
            //    if (context.RequestId != null)
            //    {
            //        await context.RespondAsync<OrderSubmissionRejected>(new
            //        {
            //            InVar.Timestamp,
            //            context.Message.OrderId,
            //            context.Message.CustomerNumber,
            //            Reason = $"Test Customer cannot submit order:{context.Message.CustomerNumber}"

            //        });
            //    }
            //    return;
            //}

            await context.Publish<OrderSubmitted>(new
            {
                context.Message.OrderId,
                context.Message.Timestamp,
                context.Message.CustomerNumber
            });

            if (context.RequestId != null)
            {
                await context.RespondAsync<OrderSubmissionAccepted>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    context.Message.CustomerNumber

                });
            }
        }
    }
}
