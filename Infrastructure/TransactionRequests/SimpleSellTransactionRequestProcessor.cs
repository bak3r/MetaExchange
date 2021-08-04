using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.TransactionRequests
{
    public class SimpleSellTransactionRequestProcessor : ISellTransactionRequestProcessor
    {
        public List<HedgerTransaction> ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges)
        {
            throw new System.NotImplementedException();
        }
    }
}