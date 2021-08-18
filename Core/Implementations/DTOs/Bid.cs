using System;

namespace Core.Implementations.DTOs
{
    public class Bid : BidOrOrderElement, IComparable
    {
        /// <summary>
        /// Implements comparisson by price.
        /// IMPORTANT NOTE !!! Reverse sort implemented - highest price first
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object? obj)
        {
            var otherBid = (Bid)obj;
            if (otherBid.Order.Price < this.Order.Price) return -1;
            if (otherBid.Order.Price > this.Order.Price) return 1;
            if (otherBid.Order.Price == this.Order.Price)
            {
                // Prefer bids with largest amount so that number
                // of buy transactions are kept to a minimum because
                // of transaction fees
                var otherBidMultiplication = otherBid.Order.Price * otherBid.Order.Amount;
                var thisBidMultiplication = this.Order.Price * this.Order.Amount;
                if (otherBidMultiplication < thisBidMultiplication) return -1;
                if (otherBidMultiplication > thisBidMultiplication) return 1;
            }
            return 0;
        }
    }
}