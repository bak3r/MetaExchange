using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.CryptoExchanges
{
    public class DummyCryptoExchangeCreator : ICryptoExchangeCreator
    {
        /// <summary>
        // Each crypto exchange should have an order book so this method creates
        /// exchanges from orderbooks specified as parameters.
        /// Name of the exchange is the key in the dictionary.
        /// </summary>
        /// <param name="exchangeNamesWithOrderBooks"></param>
        /// <returns></returns>
        public List<CryptoExchange> CreateCryptoExchangesFromMultipleOrderBooks(Dictionary<string, OrderBook> exchangeNamesWithOrderBooks)
        {
            var cryptoExchanges = new List<CryptoExchange>();
            foreach (var orderBook in exchangeNamesWithOrderBooks)
            {
                var cryptoExchange = new CryptoExchange
                {
                    Name = orderBook.Key,
                    OrderBook = orderBook.Value,
                    BalanceEur = 10000m,
                    BalanceBtc = 1m
                };

                cryptoExchanges.Add(cryptoExchange);
            }

            return cryptoExchanges;
        }
    }
}