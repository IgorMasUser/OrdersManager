namespace OrdersManager.Contracts
{
    public interface OrderSubmitted
    {
        Guid OrderId { get;}
        DateTime TimeStamp { get;}
        string CustomerNumber { get;}
    }
}
