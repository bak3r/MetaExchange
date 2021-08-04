using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface ISellTransactionRequestProcessor
    {
        List<HedgerTransaction> ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges);
    }
}