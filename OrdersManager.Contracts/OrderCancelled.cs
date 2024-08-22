namespace OrdersManager.Contracts
{
    public interface OrderCancelled
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }
    }
}
