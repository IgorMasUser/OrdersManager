using System;

namespace OrdersManager.SharedModels
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public string? ProductName { get; set; }
        public string? CustomerNumber { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
