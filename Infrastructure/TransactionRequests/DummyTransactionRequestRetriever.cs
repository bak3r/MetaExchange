using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Implementations.Enums;
using Core.Interfaces;

namespace Infrastructure.TransactionRequests
{
    public class DummyTransactionRequestRetriever : ITransactionRequestRetriever
    {
        public List<TransactionRequest> RetrieveTransactionsForProcessing()
        {
            var transactionRequests = new List<TransactionRequest>();

            var transactionRequest = new TransactionRequest {OrderType = OrderType.Buy, TransactionAmount = 1m};

            transactionRequests.Add(transactionRequest);

            return transactionRequests;
        }
    }
}