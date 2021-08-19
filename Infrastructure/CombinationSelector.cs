using System;
using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure
{
    /// <summary>
    /// Main class with business logic for selecting bids/asks from list of bids/asks of all exchanges
    /// </summary>
    /// <typeparam name="T">Ask or Bid</typeparam>
    public class CombinationSelector<T> : ICombinationSelector<T> where T : BidOrAskElement
    {
        private readonly IExchangeBalanceTracker _exchangeBalanceTracker;

        public CombinationSelector(IExchangeBalanceTracker exchangeBalanceTracker)
        {
            _exchangeBalanceTracker = exchangeBalanceTracker;
        }

        /// <summary>
        /// Prepares a list of selected bids/asks that will satisfy the requested transaction amount
        /// </summary>
        /// <param name="transactionRequestAmount">Transaction amount that needs to be satisfied</param>
        /// <param name="orderBookBidsOrAsks">List of all bids/asks from all crypto exchanges</param>
        /// <returns>List of selected bids/asks that will be needed to satisfy hedger transactions</returns>
        public List<BidOrAskElement> PrepareListOfBidsOrAsksToSatisfyTransactionAmount(decimal transactionRequestAmount, List<T> orderBookBidsOrAsks)
        {
            if (transactionRequestAmount > 0)
            {
                var bitcoinAmountNeededForTransaction = transactionRequestAmount;
                var selectedElements = new List<BidOrAskElement>();

                orderBookBidsOrAsks.Sort();

                foreach (var oneBidOrAsk in orderBookBidsOrAsks)
                {
                    if (bitcoinAmountNeededForTransaction > 0)
                    {
                        var smallerElement = Activator.CreateInstance<T>();
                        smallerElement.Order = new Order();

                        if (oneBidOrAsk.Order.Type == "Sell")
                        {
                            var exchangeBalanceThatHasThisBid = _exchangeBalanceTracker.GetBalanceForExchange(oneBidOrAsk.CryptoExchangeName);

                            if (exchangeBalanceThatHasThisBid >= oneBidOrAsk.Order.Amount)
                            {
                                if (bitcoinAmountNeededForTransaction <= oneBidOrAsk.Order.Amount)
                                {
                                    smallerElement.Order.Amount = bitcoinAmountNeededForTransaction;
                                    bitcoinAmountNeededForTransaction = 0;
                                }
                                else
                                {
                                    bitcoinAmountNeededForTransaction -= oneBidOrAsk.Order.Amount;
                                    smallerElement.Order.Amount = oneBidOrAsk.Order.Amount;
                                }
                            }
                            else
                            {
                                if (bitcoinAmountNeededForTransaction <= exchangeBalanceThatHasThisBid)
                                {
                                    smallerElement.Order.Amount = bitcoinAmountNeededForTransaction;
                                    bitcoinAmountNeededForTransaction = 0;
                                }
                                else
                                {
                                    bitcoinAmountNeededForTransaction -= exchangeBalanceThatHasThisBid;
                                    smallerElement.Order.Amount = exchangeBalanceThatHasThisBid;
                                }
                            }

                            _exchangeBalanceTracker.ReduceBalanceForExchangeByAmount(oneBidOrAsk.CryptoExchangeName, smallerElement.Order.Amount);
                        }
                        else if (oneBidOrAsk.Order.Type == "Buy")
                        {
                            if (bitcoinAmountNeededForTransaction <= oneBidOrAsk.Order.Amount)
                            {
                                smallerElement.Order.Amount = bitcoinAmountNeededForTransaction;
                                bitcoinAmountNeededForTransaction = 0;
                            }
                            else
                            {
                                bitcoinAmountNeededForTransaction -= oneBidOrAsk.Order.Amount;
                                smallerElement.Order.Amount = oneBidOrAsk.Order.Amount;
                            }
                        }

                        smallerElement.Order.Price = oneBidOrAsk.Order.Price;
                        smallerElement.Order.Id = oneBidOrAsk.Order.Id;
                        smallerElement.Order.Type = oneBidOrAsk.Order.Type;
                        smallerElement.CryptoExchangeName = oneBidOrAsk.CryptoExchangeName;

                        if(smallerElement.Order.Amount != 0)
                            selectedElements.Add(smallerElement);
                    }
                }

                if (bitcoinAmountNeededForTransaction == 0)
                    return selectedElements;
            }

            return null;
        }
    }
}