namespace Core.Implementations.DTOs
{
    public class HedgerTransaction
    {
        public Order Order { get; set; }

        /// <summary>
        /// This property is for easier display on which exchange transaction occurs
        /// </summary>
        public string CryptoExchange { get; set; }
    }
}