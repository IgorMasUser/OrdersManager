namespace OrdersManager.SharedModels
{
    public class OrderPaymentRequest
    {
        public Guid OrderId { get; set; }
        public string CustomerNumber { get; set; }
        public bool IsPaid { get; set; }
    }

}
