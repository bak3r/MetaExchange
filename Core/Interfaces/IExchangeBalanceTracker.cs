using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IExchangeBalanceTracker
    {
        decimal GetBalanceForExchange(string exchangeName);
        void ReduceBalanceForExchangeByAmount(string cryptoExchangeName, decimal reduceBalanceByAmount);
        void SetUpInitialExchangeBalances(List<CryptoExchange> cryptoExchanges);
    }
}