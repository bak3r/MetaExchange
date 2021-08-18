using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure
{
    public class CombinationSelector<T> : ICombinationSelector<T> where T : BidOrOrderElement
    {
        public List<T> PrepareListOfBidsOrAsksToSatisfyTransactionAmount(decimal transactionRequestAmount, List<T> orderBookBidsOrAsks)
        {
            if (transactionRequestAmount > 0)
            {
                var bitcoinAmountNeededForTransaction = transactionRequestAmount;
                var selectedElements = new List<T>();

                orderBookBidsOrAsks.Sort();

                foreach (var oneBidOrAsk in orderBookBidsOrAsks)
                {
                    if (bitcoinAmountNeededForTransaction > 0)
                    {
                        BidOrOrderElement smallerElement;
                        if (typeof(T) == typeof(Bid))
                        {
                            smallerElement = new Bid();
                        }
                        else if (typeof(T) == typeof(Ask))
                        {
                            smallerElement = new Ask();
                        }
                        else
                        {
                            return null;
                        }
                        
                        smallerElement.Order = new Order();

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

                        smallerElement.Order.Price = oneBidOrAsk.Order.Price;
                        smallerElement.Order.Id = oneBidOrAsk.Order.Id;
                        smallerElement.Order.Type = oneBidOrAsk.Order.Type;

                        selectedElements.Add(smallerElement as T);
                    }

                }

                if (bitcoinAmountNeededForTransaction == 0)
                    return selectedElements;
            }

            return null;
        }
    }
}