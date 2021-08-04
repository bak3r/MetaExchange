using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Terminal
{
    public class BaseTransactionProcessor
    {
        private readonly IOrderBookRetriever _orderBookRetriever;
        private readonly ICryptoExchangeCreator _cryptoExchangeCreator;
        private readonly ICryptoExchangePresenter _cryptoExchangePresenter;
        private readonly ITransactionRequestRetriever _transactionRequestRetriever;
        private readonly ITransactionRequestPresenter _transactionRequestPresenter;

        public BaseTransactionProcessor(IOrderBookRetriever orderBookRetriever, ICryptoExchangeCreator cryptoExchangeCreator, ICryptoExchangePresenter cryptoExchangePresenter, ITransactionRequestRetriever transactionRequestRetriever, ITransactionRequestPresenter transactionRequestPresenter)
        {
            _orderBookRetriever = orderBookRetriever;
            _cryptoExchangeCreator = cryptoExchangeCreator;
            _cryptoExchangePresenter = cryptoExchangePresenter;
            _transactionRequestRetriever = transactionRequestRetriever;
            _transactionRequestPresenter = transactionRequestPresenter;
        }

        public void Run()
        {
            var orderBooks = _orderBookRetriever.RetrieveOrderBooks(2);

            var cryptoExchanges = _cryptoExchangeCreator.CreateCryptoExchangesFromMultipleOrderBooks(orderBooks);
            _cryptoExchangePresenter.OutputCryptoExchangesInfo(cryptoExchanges);

            var transactionRequests = _transactionRequestRetriever.RetrieveTransactionsForProcessing();
            foreach (var transactionRequest in transactionRequests)
            {
                _transactionRequestPresenter.DisplayTransactionRequestInfo(transactionRequest);
            }
        }
    }
}