using OrdersManager.Data.Abstraction;
using OrdersManager.Models;

namespace OrdersManager.Data.Implementation
{
    public class OrderRepository : IOrderRepository<Order>
    {
        private readonly List<Order> orders = new();

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await Task.FromResult(orders);
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            var order = orders.Find(o => o.OrderId == id);
            if (order != null)
            {
                return await Task.FromResult(order);
            }
            else
            {
                return await Task.FromResult<Order>(null);
            }

        }

        public async Task AddAsync(Order entity)
        {
            orders.Add(entity);
            await Task.CompletedTask;
        }
    }
}
