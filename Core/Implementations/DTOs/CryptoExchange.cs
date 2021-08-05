using System.Collections.Generic;

namespace Core.Implementations.DTOs
{
    public class CryptoExchange
    {
        public string Name { get; set; }
        public OrderBook OrderBook { get; set; }
        public decimal BalanceEur { get; set; }
        public decimal BalanceBtc { get; set; }

        public CryptoExchange()
        {
            OrderBook = new OrderBook {Bids = new List<Bid>(), Asks = new List<Ask>()};
        }
    }
}