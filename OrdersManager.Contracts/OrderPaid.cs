namespace OrdersManager.Contracts
{
    public interface OrderPaid
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }
    }
}
