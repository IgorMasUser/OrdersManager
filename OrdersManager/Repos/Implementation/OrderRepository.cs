using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

        public async Task AddAsync(Order order)
        {
            try
            {
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task DeleteAll()
        {
            var listOfOrders = await context.Orders.Include(o => o.OrderStatus).ToListAsync();
            var listOfStates = await context.OrderStates.ToListAsync();
            if (listOfOrders.Count > 0)
            {
                context.RemoveRange(listOfOrders);

            }
            if (listOfStates.Count > 0)
            {
                context.RemoveRange(listOfStates);

            }

            await context.SaveChangesAsync();

        }

        public async Task SaveAllChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<Order> GetByIdAsync(string CustomerId)
        {
            var order = await context.Orders.Where(o=>o.CustomerNumber.Equals(CustomerId)).FirstOrDefaultAsync();
            if(order != null)
            {
                return order;
            }
            else
            {
                return null;
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }
    }
}
