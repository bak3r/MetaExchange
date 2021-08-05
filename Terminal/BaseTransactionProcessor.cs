using System;
using System.Globalization;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Terminal
{
    public class BaseTransactionProcessor
    {
        private readonly IOrderBookRetriever _orderBookRetriever;
        private readonly ICryptoExchangeCreator _cryptoExchangeCreator;
        private readonly ICryptoExchangePresenter _cryptoExchangePresenter;
        private readonly ITransactionRequestRetriever _transactionRequestRetriever;
        private readonly ITransactionRequestPresenter _transactionRequestPresenter;
        private readonly ITransactionRequestProcessor _transactionRequestProcessor;
        private readonly IHedgerTransactionPresenter _hedgerTransactionPresenter;
        private readonly IConfiguration _configuration;

        public BaseTransactionProcessor(IOrderBookRetriever orderBookRetriever,
            ICryptoExchangeCreator cryptoExchangeCreator, ICryptoExchangePresenter cryptoExchangePresenter,
            ITransactionRequestRetriever transactionRequestRetriever,
            ITransactionRequestPresenter transactionRequestPresenter,
            ITransactionRequestProcessor transactionRequestProcessor,
            IHedgerTransactionPresenter hedgerTransactionPresenter,
            IConfiguration configuration)
        {
            _orderBookRetriever = orderBookRetriever;
            _cryptoExchangeCreator = cryptoExchangeCreator;
            _cryptoExchangePresenter = cryptoExchangePresenter;
            _transactionRequestRetriever = transactionRequestRetriever;
            _transactionRequestPresenter = transactionRequestPresenter;
            _transactionRequestProcessor = transactionRequestProcessor;
            _hedgerTransactionPresenter = hedgerTransactionPresenter;
            _configuration = configuration;
        }

        public void Run()
        {
            bool nrOrderBooksParsedSuccessfully = int.TryParse(_configuration["OrderBooks:NumeberOfOrderbooksToReadFromFile"], out var parsedNumberOfOrderbooksToRetrieve);
            if (!nrOrderBooksParsedSuccessfully)
                parsedNumberOfOrderbooksToRetrieve = 1;
            var orderBooks = _orderBookRetriever.RetrieveOrderBooks(parsedNumberOfOrderbooksToRetrieve);

            var cryptoExchanges = _cryptoExchangeCreator.CreateCryptoExchangesFromMultipleOrderBooks(orderBooks);
            _cryptoExchangePresenter.OutputCryptoExchangesInfo(cryptoExchanges);

            var transactionRequests = _transactionRequestRetriever.RetrieveTransactionsForProcessing();
            foreach (var transactionRequest in transactionRequests)
            {
                _transactionRequestPresenter.DisplayTransactionRequestInfo(transactionRequest);

                var processorResult = _transactionRequestProcessor.ProcessTransaction(transactionRequest, cryptoExchanges);

                if(processorResult.TransactionIsValid)
                    _hedgerTransactionPresenter.DisplayHedgerTransactions(processorResult.HedgerTransactions);
                else
                {
                    Console.WriteLine("#### Error message ##############################################");
                    Console.WriteLine("Hedger transactions were not generated. Reason: " +
                                      processorResult.ErrorMessage);
                }
            }
        }
    }
}