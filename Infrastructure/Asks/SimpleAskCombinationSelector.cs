using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.Asks
{
    public class SimpleAskCombinationSelector : IAskCombinationSelector
    {
        public List<Ask> PrepareListOfAsksToSatisfyTransactionAmount(decimal transactionRequestAmount, List<Ask> orderBookAsks)
        {
            if (transactionRequestAmount > 0)
            {
                var bitcoinAmountNeeded = transactionRequestAmount;
                var selectedAsks = new List<Ask>();

                orderBookAsks.Sort();

                foreach (var ask in orderBookAsks)
                {
                    if (bitcoinAmountNeeded > 0)
                    {
                        var smallerAsk = new Ask();
                        smallerAsk.Order = new Order();

                        if (bitcoinAmountNeeded <= ask.Order.Amount)
                        {
                            smallerAsk.Order.Amount = bitcoinAmountNeeded;
                            bitcoinAmountNeeded = 0;
                        }
                        else
                        {
                            bitcoinAmountNeeded -= ask.Order.Amount;
                            smallerAsk.Order.Amount = ask.Order.Amount;
                        }

                        smallerAsk.Order.Price = ask.Order.Price;
                        smallerAsk.Order.Id = ask.Order.Id;
                        smallerAsk.Order.Type = ask.Order.Type;

                        selectedAsks.Add(smallerAsk);
                    }
                }

                if (bitcoinAmountNeeded == 0)
                    return selectedAsks;
            }

            return null;
        }
    }
}