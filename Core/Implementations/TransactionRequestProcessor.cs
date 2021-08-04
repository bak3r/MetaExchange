using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Implementations.Enums;
using Core.Interfaces;

namespace Core.Implementations
{
    public class TransactionRequestProcessor : ITransactionRequestProcessor
    {
        private readonly IBuyTransactionRequestProcessor _buyTransactionRequestProcessor;
        private readonly ISellTransactionRequestProcessor _sellTransactionRequestProcessor;

        public TransactionRequestProcessor(IBuyTransactionRequestProcessor buyTransactionRequestProcessor, ISellTransactionRequestProcessor sellTransactionRequestProcessor)
        {
            _buyTransactionRequestProcessor = buyTransactionRequestProcessor;
            _sellTransactionRequestProcessor = sellTransactionRequestProcessor;
        }

        public List<HedgerTransaction> ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges)
        {
            switch (transactionRequest.OrderType)
            {
                case OrderType.Buy:
                    var buyRequestProcessorResult = _buyTransactionRequestProcessor.ProcessTransaction(transactionRequest, cryptoExchanges);
                    if(buyRequestProcessorResult.TransactionIsValid)
                        return buyRequestProcessorResult.HedgerTransactions;
                    break;
                case OrderType.Sell:
                    var sellHedgerTransactions = _sellTransactionRequestProcessor.ProcessTransaction(transactionRequest, cryptoExchanges);
                    return sellHedgerTransactions;
            }

            return null;
        }
    }
}