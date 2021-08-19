using System;

namespace Core.Implementations.DTOs
{
    public class Ask : BidOrAskElement, IComparable
    {
        public int CompareTo(object? obj)
        {
            var otherAsk = (Ask)obj;
            if (this.Order.Price < otherAsk.Order.Price) return -1;
            if (this.Order.Price > otherAsk.Order.Price) return 1;
            if (otherAsk.Order.Price == this.Order.Price)
            {
                // Prefer asks with largest amount so that number
                // of sell transactions are kept to a minimum because
                // of transaction fees
                var otherAskMultiplication = otherAsk.Order.Price * otherAsk.Order.Amount;
                var thisAskMultiplication = this.Order.Price * this.Order.Amount;
                if (otherAskMultiplication < thisAskMultiplication) return 1;
                if (otherAskMultiplication > thisAskMultiplication) return -1;
            }
            return 0;
        }
    }
}