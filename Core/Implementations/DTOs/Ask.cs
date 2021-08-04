using System;

namespace Core.Implementations.DTOs
{
    public class Ask : IComparable
    {
        public Order Order { get; set; }
        public int CompareTo(object? obj)
        {
            var otherAsk = (Ask)obj;
            if (this.Order.Price < otherAsk.Order.Price) return -1;
            if (this.Order.Price > otherAsk.Order.Price) return 1;
            return 0;
        }
    }
}