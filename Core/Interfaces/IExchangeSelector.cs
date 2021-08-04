using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IExchangeSelector
    {
        (string, List<Ask>) FindExchangeWithLowestPossibleAskTransactionCost(Dictionary<string, List<Ask>> tradeableExchangesWithFilteredAskLists);
    }
}