using Core.Implementations.DTOs;
using Core.Implementations.Enums;
using Core.Interfaces;

namespace Infrastructure.TransactionRequests
{
    public class TerminalTransactionRequestPresenter : ITransactionRequestPresenter
    {
        public void DisplayTransactionRequestInfo(TransactionRequest transactionRequest)
        {
            System.Console.WriteLine("#### Requested transaction info #################################");
            System.Console.WriteLine("Transaction order type: " + transactionRequest.OrderType);
            System.Console.WriteLine("Transaction amount: " + transactionRequest.TransactionAmount);
        }
    }
}