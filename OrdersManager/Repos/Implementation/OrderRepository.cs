using Microsoft.EntityFrameworkCore;
using OrdersManager.Data.Abstraction;
using OrdersManager.Models;

namespace OrdersManager.Data.Implementation
{
    public class OrderRepository : IOrderRepository<Order>
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var listOfOrders = await context.Orders
                                            .Include(o => o.Items)
                                            .Include(o => o.OrderStatus)
                                            .ToListAsync();
            return listOfOrders;
        }

        public async Task AddAsync(Order order)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            var listOfOrders = await context.Orders.ToListAsync();
            if (listOfOrders.Count > 0)
            {
                context.RemoveRange(listOfOrders);
                await context.SaveChangesAsync();
            }

        }
    }
}
