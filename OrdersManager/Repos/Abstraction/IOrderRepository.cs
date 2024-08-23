namespace OrdersManager.Data.Abstraction
{
    public interface IOrderRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task SaveAllChangesAsync();
        Task DeleteAll();
        Task<TEntity> GetByIdAsync(string id);
    }
}
