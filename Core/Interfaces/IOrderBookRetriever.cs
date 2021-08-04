using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IOrderBookRetriever
    {
        Dictionary<string, OrderBook> RetrieveOrderBooks(int numberOfOrderBooksToRetrieve);
    }
}