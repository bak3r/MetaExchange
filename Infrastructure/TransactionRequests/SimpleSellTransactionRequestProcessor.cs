using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.TransactionRequests
{
    public class SimpleSellTransactionRequestProcessor : ISellTransactionRequestProcessor
    {
        public RequestProcessorResult ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges)
        {
            if (transactionRequest.TransactionAmount > 0)
            {
                if (cryptoExchanges.Any())
                {
                    var exchangesWithNonEmptyBidList = (from ce in cryptoExchanges
                        where ce.OrderBook.Bids.Count > 0
                        select ce).ToList();

                    if (exchangesWithNonEmptyBidList.Count > 0)
                    {
                        var dummyExchange = exchangesWithNonEmptyBidList[0];

                        if (dummyExchange.OrderBook.Bids[0].Order.Amount >= transactionRequest.TransactionAmount)
                        {
                            return new RequestProcessorResult()
                                { TransactionIsValid = true };
                        }

                        return new RequestProcessorResult()
                            { TransactionIsValid = false, ErrorMessage = "No crypto exchanges with bid high enough to satisfy the request exist." };
                    }

                    return new RequestProcessorResult()
                        { TransactionIsValid = false, ErrorMessage = "No crypto exchanges with non-empty bid list exist." };
                }
                else
                {
                    return new RequestProcessorResult()
                        { TransactionIsValid = false, ErrorMessage = "No crypto exchanges exist." };
                }
            }

            return new RequestProcessorResult()
                {TransactionIsValid = false, ErrorMessage = "Transaction amount must be larger than 0."};
        }
        
    }
}