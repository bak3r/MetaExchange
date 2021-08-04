using System;

namespace Core.Implementations.DTOs
{
    public class Bid : IComparable
    {
        public Order Order { get; set; }
        public int CompareTo(object? obj)
        {
            var otherBid = (Bid)obj;
            if (this.Order.Price < otherBid.Order.Price) return -1;
            if (this.Order.Price > otherBid.Order.Price) return 1;
            return 0;
        }
    }
}