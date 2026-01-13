namespace OrderApp
{
    public interface IOrderRepository
    {
        void Save(Order order);
    }

    public interface IPaymentGateway
    {
        bool Charge(decimal amount);
    }

    public class Order
    {
        public decimal Total { get; set; }
        public bool IsPaid { get; set; }
    }
}
namespace OrderApp
{
    public class OrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IPaymentGateway _payment;

        public OrderService(IOrderRepository repo, IPaymentGateway payment)
        {
            _repo = repo;
            _payment = payment;
        }

        public bool PlaceOrder(Order order)
        {
            var success = _payment.Charge(order.Total);
            if (!success) return false;

            order.IsPaid = true;
            _repo.Save(order);
            return true;
        }
    }
}