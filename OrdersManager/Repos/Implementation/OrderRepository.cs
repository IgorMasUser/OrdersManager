using Microsoft.EntityFrameworkCore;
using OrdersManager.Data.Abstraction;
using OrdersManager.SharedModels;

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

        public void AddAsync(Order order)
        {
            context.Orders.Add(order);
        }

        public async Task DeleteAll()
        {
            var listOfOrders = await context.Orders.Include(o => o.OrderStatus).ToListAsync();
            if (listOfOrders.Count > 0)
            {
                var listOfOrderStatuses = listOfOrders.Select(o => o.OrderStatus).ToList();
                context.RemoveRange(listOfOrderStatuses);

                context.RemoveRange(listOfOrders);

                await context.SaveChangesAsync();
            }

        }

        public async Task SaveAllChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
