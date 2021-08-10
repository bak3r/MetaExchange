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

        public (string, List<Bid>) FindExchangeWithHighestPossibleBidTransactionValue(Dictionary<string, List<Bid>> tradeableExchangesWithSelectedBids)
        {
            var exchangesWithTransactionCostSum = new List<KeyValuePair<string, decimal>>();

            foreach (var singleExchangeWithBids in tradeableExchangesWithSelectedBids)
            {
                decimal exchangeBidSumCost = new decimal();
                foreach (var bid in singleExchangeWithBids.Value)
                {
                    var singleBidSumCost = bid.Order.Amount * bid.Order.Price;
                    exchangeBidSumCost += singleBidSumCost;
                }

                if (exchangeBidSumCost > 0)
                {
                    exchangesWithTransactionCostSum.Add(
                        new KeyValuePair<string, decimal>(singleExchangeWithBids.Key, exchangeBidSumCost));
                }
            }

            exchangesWithTransactionCostSum.Sort(
                delegate (KeyValuePair<string, decimal> firstPair, KeyValuePair<string, decimal> secondPair)
                {
                    return secondPair.Value.CompareTo(firstPair.Value);
                });

            if (exchangesWithTransactionCostSum.Any())
            {
                return (exchangesWithTransactionCostSum[0].Key,
                    tradeableExchangesWithSelectedBids[exchangesWithTransactionCostSum[0].Key]);
            }

            return (null, null);
        }
    }
}