using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.Bids
{
    public class SimpleBidCombinationSelector : IBidCombinationSelector
    {
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