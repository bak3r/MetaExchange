using System.Collections.Generic;
using System.Linq;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.CryptoExchanges
{
    public class SimpleExchangeSelector : IExchangeSelector
    {
        public (string, List<Ask>) FindExchangeWithLowestPossibleAskTransactionCost(Dictionary<string, List<Ask>> tradableExchangesWithFilteredAskLists)
        {
            var exchangesWithTransactionCostSum = new List<KeyValuePair<string, decimal>>();

            foreach (var singleExchangeWithAsks in tradableExchangesWithFilteredAskLists)
            {
                decimal exchangeAskSumCost = new decimal();
                foreach (var ask in singleExchangeWithAsks.Value)
                {
                    var singleAskSumCost = ask.Order.Amount * ask.Order.Price;
                    exchangeAskSumCost += singleAskSumCost;
                }

                if (exchangeAskSumCost > 0)
                {
                    exchangesWithTransactionCostSum.Add(
                        new KeyValuePair<string, decimal>(singleExchangeWithAsks.Key, exchangeAskSumCost));
                }
            }

            exchangesWithTransactionCostSum.Sort(
                delegate (KeyValuePair<string, decimal> firstPair, KeyValuePair<string, decimal> secondPair)
                {
                    return firstPair.Value.CompareTo(secondPair.Value);
                });

            if (exchangesWithTransactionCostSum.Any())
            {
                return (exchangesWithTransactionCostSum[0].Key,
                    tradableExchangesWithFilteredAskLists[exchangesWithTransactionCostSum[0].Key]);
            }

            return (null, null);
        }
    }
}