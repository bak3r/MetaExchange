using System.Collections.Generic;
using System.Linq;
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