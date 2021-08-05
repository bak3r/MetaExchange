using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Implementations.DTOs;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.CryptoExchanges
{
    public class DummyCryptoExchangeCreator : ICryptoExchangeCreator
    {
        readonly decimal _exchange1BtcBalance;
        readonly decimal _exchange2BtcBalance;

        public DummyCryptoExchangeCreator(IConfiguration configuration)
        {
            // This is meant for app testing so that you can set balance on first two exchnages/orderbooks in file.
            decimal.TryParse(configuration["CryptoExchanges:CryptoExchange1BtcBalance"], NumberStyles.Float,
                CultureInfo.CreateSpecificCulture("en-GB"), out _exchange1BtcBalance);
            decimal.TryParse(configuration["CryptoExchanges:CryptoExchange2BtcBalance"], NumberStyles.Float,
                CultureInfo.CreateSpecificCulture("en-GB"), out _exchange2BtcBalance);
        }

        /// <summary>
        /// Each crypto exchange should have an order book so this method creates
        /// exchanges from orderbooks specified as parameters.
        /// Name of the exchange is the key in the dictionary.
        /// For first two exchanges the bitcoin balance can be set in the configuration file so that
        /// the application can be tested.
        /// </summary>
        /// <param name="exchangeNamesWithOrderBooks">Dictionary of CryptoExchangeName/Orderbook entries</param>
        /// <returns>List of generated CryptoExchanges</returns>
        public List<CryptoExchange> CreateCryptoExchangesFromMultipleOrderBooks(Dictionary<string, OrderBook> exchangeNamesWithOrderBooks)
        {
            var cryptoExchanges = new List<CryptoExchange>();

            int counter = 0;
            foreach (var orderBook in exchangeNamesWithOrderBooks)
            {
                
                var cryptoExchange = new CryptoExchange
                {
                    Name = orderBook.Key,
                    OrderBook = orderBook.Value,
                    BalanceEur = 10000m,
                    BalanceBtc = 1.5m
                };

                // For app testing (see method comment above)
                if (counter == 0)
                    cryptoExchange.BalanceBtc = _exchange1BtcBalance;
                else if (counter == 1)
                    cryptoExchange.BalanceBtc = _exchange2BtcBalance;

                cryptoExchanges.Add(cryptoExchange);
                counter++;
            }

            return cryptoExchanges;
        }
    }
}