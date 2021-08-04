using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface ITransactionRequestRetriever
    {
        List<TransactionRequest> RetrieveTransactionsForProcessing();
    }
}