using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface ITransactionRequestPresenter
    {
        void DisplayTransactionRequestInfo(TransactionRequest transactionRequest);
    }
}