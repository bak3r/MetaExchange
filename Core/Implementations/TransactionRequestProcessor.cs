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

        public RequestProcessorResult ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges)
        {
            switch (transactionRequest.OrderType)
            {
                case OrderType.Buy:
                    var buyRequestProcessorResult = _buyTransactionRequestProcessor.ProcessTransaction(transactionRequest, cryptoExchanges);
                    return buyRequestProcessorResult;
                case OrderType.Sell:
                    var sellRequestProcessorResult = _sellTransactionRequestProcessor.ProcessTransaction(transactionRequest, cryptoExchanges);
                    return sellRequestProcessorResult;
            }

            return null;
        }
    }
}