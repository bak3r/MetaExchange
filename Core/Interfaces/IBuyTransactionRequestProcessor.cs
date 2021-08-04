using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IBuyTransactionRequestProcessor
    {
        List<HedgerTransaction> ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges);
    }
}