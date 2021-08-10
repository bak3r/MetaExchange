using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.Bids
{
    /// <summary>
    /// Selects best bids from a list of bids to satisfy transaction amount
    /// </summary>
    public class SimpleBidCombinationSelector : IBidCombinationSelector
    {
        /// <summary>
        /// Prepares a list of selected bids that will yield biggest value gain for
        /// specified transaction amount. It uses reverse sorting for comparison between
        /// bids. See class Bid.
        /// </summary>
        /// <param name="transactionRequestAmount">Amount requested by transaction</param>
        /// <param name="orderBookBids">List of best bids for transaction</param>
        /// <returns></returns>
        public List<Bid> PrepareListOfBidsToSatisfyTransactionAmount(decimal transactionRequestAmount, List<Bid> orderBookBids)
        {
            if (transactionRequestAmount > 0)
            {
                var bitcoinAmountNeededToSell = transactionRequestAmount;
                var selectedBids = new List<Bid>();

                orderBookBids.Sort();

                foreach (var bid in orderBookBids)
                {
                    if (bitcoinAmountNeededToSell > 0)
                    {
                        var smallerBid = new Bid();
                        smallerBid.Order = new Order();

                        if (bitcoinAmountNeededToSell <= bid.Order.Amount)
                        {
                            smallerBid.Order.Amount = bitcoinAmountNeededToSell;
                            bitcoinAmountNeededToSell = 0;
                        }
                        else
                        {
                            bitcoinAmountNeededToSell -= bid.Order.Amount;
                            smallerBid.Order.Amount = bid.Order.Amount;
                        }

                        smallerBid.Order.Price = bid.Order.Price;
                        smallerBid.Order.Id = bid.Order.Id;
                        smallerBid.Order.Type = bid.Order.Type;

                        selectedBids.Add(smallerBid);
                    }
                    
                }

                if (bitcoinAmountNeededToSell == 0)
                    return selectedBids;
            }

            return null;
        }
    }
}