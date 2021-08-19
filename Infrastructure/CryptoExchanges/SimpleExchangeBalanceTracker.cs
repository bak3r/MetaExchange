using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.CryptoExchanges
{
    /// <summary>
    /// Intended as simple implementation of an outside service that keeps track of balance
    /// on various crypto exchanges.
    /// </summary>
    public class SimpleExchangeBalanceTracker : IExchangeBalanceTracker
    {
        /// <summary>
        /// Dictionary of exchangeNames with their respective balances
        /// </summary>
        Dictionary<string, decimal> exchangeBalances;

        public SimpleExchangeBalanceTracker()
        {
            exchangeBalances = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// Retrieves balance for specific cryptoExchange
        /// Note - since this is a simple implementation there is no
        /// checks for getting balance from exchangeName that is not
        /// in the dictionary
        /// </summary>
        /// <param name="exchangeName">Exchange name</param>
        /// <returns>MetaExchange's bitcoin balance on specified exchange</returns>
        public decimal GetBalanceForExchange(string exchangeName)
        {
            return exchangeBalances[exchangeName];
        }

        /// <summary>
        /// Reduces balance on specified crypto exchange by amount
        /// Note - no checks for trying to reduce amount that is larger than current balance
        /// </summary>
        /// <param name="cryptoExchangeName">Crypto exchange that needs to have balance reduced</param>
        /// <param name="reduceBalanceByAmount">Amount of bitcoin that needs to be reduced from balance</param>
        public void ReduceBalanceForExchangeByAmount(string cryptoExchangeName, decimal reduceBalanceByAmount)
        {
            var balance = exchangeBalances[cryptoExchangeName];
            balance -= reduceBalanceByAmount;
            exchangeBalances[cryptoExchangeName] = balance;
        }

        /// <summary>
        /// Initialize dictionary with balances from read crypto exchanges that were created from
        /// order books/configuration
        /// </summary>
        /// <param name="cryptoExchanges"></param>
        public void SetUpInitialExchangeBalances(List<CryptoExchange> cryptoExchanges)
        {
            foreach (var cryptoExchange in cryptoExchanges)
            {
                exchangeBalances.Add(cryptoExchange.Name, cryptoExchange.BalanceBtc);
            }
        }
    }
}