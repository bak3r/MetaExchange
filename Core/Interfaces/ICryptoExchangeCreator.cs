using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface ICryptoExchangeCreator
    {
        List<CryptoExchange> CreateCryptoExchangesFromMultipleOrderBooks(Dictionary<string, OrderBook> exchangeNamesWithOrderBooks);
    }
}