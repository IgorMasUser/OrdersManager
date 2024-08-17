﻿using OrdersManager.Components.StateMachines;

namespace OrdersManager.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderState OrderStatus { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
