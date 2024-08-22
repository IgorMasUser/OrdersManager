namespace OrdersManager.Data.Abstraction
{
    public interface IOrderRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        void AddAsync(TEntity entity);
        Task SaveAllChangesAsync();
        Task DeleteAll();
    }
}
