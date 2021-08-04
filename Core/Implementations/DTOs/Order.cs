using System;

namespace Core.Implementations.DTOs
{
    public class Order
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        /// <summary>
        /// "Buy" or "Sell"
        /// </summary>
        public string Type { get; set; }
        public string Kind { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
    }
}