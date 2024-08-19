using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrdersManager.Contracts;
using OrdersManager.Data.Abstraction;
using OrdersManager.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OrdersManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository<Order> orderRepository;
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IRequestClient<SubmitOrder> submitOrderRequestClient;
        private readonly IRequestClient<CheckOrder> checkOrderClient;

        public OrdersController(IOrderRepository<Order> orderRepository, ISendEndpointProvider sendEndpointProvider, IRequestClient<SubmitOrder> submitOrderRequestClient,
            IRequestClient<CheckOrder> checkOrderClient)
        {
            this.orderRepository = orderRepository;
            this.sendEndpointProvider = sendEndpointProvider;
            this.submitOrderRequestClient = submitOrderRequestClient;
            this.checkOrderClient = checkOrderClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            await orderRepository.AddAsync(order);

            var (accepted, rejected) = await submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = order.OrderStatus.CorrelationId,
                InVar.Timestamp,
                CustomerNumber = order.CustomerNumber
            });
            if (accepted.IsCompletedSuccessfully)
            {
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

        //[HttpPost("payment")]
        //public async Task<IActionResult> OrderPayment([FromBody] OrderPaymentRequest request)
        //{
        //    var order = await _orderRepository.GetByIdAsync(request.OrderId);

        //    if (order == null)
        //        return NotFound();

        //    if (request.IsPaid)
        //    {
        //        await _publishEndpoint.Publish(new OrderPaid { OrderId = request.OrderId });
        //    }
        //    else
        //    {
        //        await _publishEndpoint.Publish(new OrderCancelled { OrderId = request.OrderId });
        //    }

        //    return Ok();
        //}
    }
}
