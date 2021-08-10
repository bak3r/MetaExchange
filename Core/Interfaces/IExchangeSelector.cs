using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IExchangeSelector
    {
        (string, List<Ask>) FindExchangeWithLowestPossibleAskTransactionCost(Dictionary<string, List<Ask>> tradeableExchangesWithFilteredAskLists);
        (string, List<Bid>) FindExchangeWithHighestPossibleBidTransactionValue(Dictionary<string, List<Bid>> tradeableExchangesWithSelectedBids);
    }
}