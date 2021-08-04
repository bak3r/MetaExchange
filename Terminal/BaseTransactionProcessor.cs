using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Terminal
{
    public class BaseTransactionProcessor
    {
        private readonly IOrderBookRetriever _orderBookRetriever;
        private readonly ICryptoExchangeCreator _cryptoExchangeCreator;
        private readonly ICryptoExchangePresenter _cryptoExchangePresenter;

        public BaseTransactionProcessor(IOrderBookRetriever orderBookRetriever, ICryptoExchangeCreator cryptoExchangeCreator, ICryptoExchangePresenter cryptoExchangePresenter)
        {
            _orderBookRetriever = orderBookRetriever;
            _cryptoExchangeCreator = cryptoExchangeCreator;
            _cryptoExchangePresenter = cryptoExchangePresenter;
        }

        public void Run()
        {
            var orderBooks = _orderBookRetriever.RetrieveOrderBooks(2);

            var cryptoExchanges = _cryptoExchangeCreator.CreateCryptoExchangesFromMultipleOrderBooks(orderBooks);

            _cryptoExchangePresenter.OutputCryptoExchangesInfo(cryptoExchanges);
        }


        
    }
}