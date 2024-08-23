namespace OrdersManager.SharedModels
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? CustomerNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid OrderStateId { get; set; } 
        public OrderState OrderStatus { get; set; } = new OrderState(); 
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

}
