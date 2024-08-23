using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrdersManager.Contracts;
using OrdersManager.Data.Abstraction;
using System.Text.Json.Serialization;
using System.Text.Json;
using OrdersManager.SharedModels;

namespace OrdersManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository<Order> orderRepository;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IRequestClient<SubmitOrder> submitOrderRequestClient;
        private readonly IRequestClient<CheckOrder> checkOrderClient;

        public OrdersController(IOrderRepository<Order> orderRepository, IPublishEndpoint publishEndpoint, IRequestClient<SubmitOrder> submitOrderRequestClient,
            IRequestClient<CheckOrder> checkOrderClient)
        {
            this.orderRepository = orderRepository;
            this.publishEndpoint = publishEndpoint;
            this.submitOrderRequestClient = submitOrderRequestClient;
            this.checkOrderClient = checkOrderClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            await orderRepository.AddAsync(order);
            await orderRepository.SaveAllChangesAsync();

            var (accepted, rejected) = await submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = order.OrderStatus.CorrelationId,
                InVar.Timestamp,
                CustomerNumber = order.CustomerNumber
            });
            if (accepted.IsCompletedSuccessfully)
            {
                await orderRepository.SaveAllChangesAsync();

                var response = await accepted;
                return Ok(response);
            }

            else
            {
                var response = await rejected;
                return BadRequest(response.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await orderRepository.GetAllAsync();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(orders, options);
            return Ok(json);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllOrders()
        {
            await orderRepository.DeleteAll();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> OrderPayment([FromBody] OrderPaymentRequest request)
        {
            var order = await orderRepository.GetByIdAsync(request.CustomerNumber);

            if (order == null)
                return NotFound();

            if (request.IsPaid)
            {
                await publishEndpoint.Publish<OrderPaid>(new 
                { 
                    OrderId = request.OrderId,
                    TimeStamp = InVar.Timestamp
                });
            }
            else
            {

                await publishEndpoint.Publish<OrderCancelled>( new  
                { 
                    OrderId = request.OrderId,
                    TimeStamp = InVar.Timestamp
                });
            }

            return Ok();
        }
    }
}
