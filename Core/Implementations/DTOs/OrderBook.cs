using System;
using System.Collections.Generic;

namespace Core.Implementations.DTOs
{
    public class OrderBook
    {
        public DateTime AcqTime { get; set; }
        public List<Bid> Bids { get; set; }
        public List<Ask> Asks { get; set; }

        public OrderBook()
        {
            Bids = new List<Bid>();
            Asks = new List<Ask>();
        }
    }
}