using Core.Implementations.Enums;

namespace Core.Implementations.DTOs
{
    public class TransactionRequest
    {
        public OrderType OrderType { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}