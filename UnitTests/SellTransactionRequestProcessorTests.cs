using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Implementations.Enums;
using Core.Interfaces;
using Infrastructure.TransactionRequests;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class SellTransactionRequestProcessorTests 
    {
        [Test]
        public void T01_ProcessTransaction_TransactionRequestIsNotHigherThanZero_TransactionIsInvalid()
        {
            var sellTransactionRequestProcessor = CreateSellTransactionRequestProcessor();
            var stubTransactionRequest = new TransactionRequest {TransactionAmount = 0m, OrderType = OrderType.Sell};

            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, new List<CryptoExchange>());

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("Transaction amount must be larger than 0.", result.ErrorMessage);
        }

        [Test]
        public void T02_ProcessTransaction_ExchangesListIsEmpty_TransactionIsInvalid()
        {
            var sellTransactionRequestProcessor = CreateSellTransactionRequestProcessor();
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = 1m, OrderType = OrderType.Sell };

            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, new List<CryptoExchange>());

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("No crypto exchanges exist.", result.ErrorMessage);
        }

        [Test]
        public void T03_ProcessTransaction_OneExchangeExistsButHasNoBids_TransactionIsInvalid()
        {
            var sellTransactionRequestProcessor = CreateSellTransactionRequestProcessor();
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = 1m, OrderType = OrderType.Sell };

            var stubCryptoExchanges = new List<CryptoExchange>()
            {
                new CryptoExchange() {Name = "StubCryptoExchange", OrderBook = new OrderBook() {Bids = new List<Bid>()}}
            };
            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("No crypto exchanges with non-empty bid list exist.", result.ErrorMessage);
        }

        [Test]
        public void T04_ProcessTransaction_OneExchangeExistsWithOneBidThatIsLowerThanRequestedTransaction_TransactionIsInvalid()
        {
            var sellTransactionRequestProcessor = CreateSellTransactionRequestProcessor();
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = 1m, OrderType = OrderType.Sell };

            var stubBid = new Bid {Order = new Order() {Amount = 0.5m}};
            var stubCryptoExchanges = new List<CryptoExchange>()
            {
                new CryptoExchange() {Name = "StubCryptoExchange", OrderBook = new OrderBook() {Bids = new List<Bid>()
                    {
                        stubBid
                    }}}
            };
            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("No crypto exchanges with bid high enough to satisfy the request exist.",
                result.ErrorMessage);
        }

        #region HelperMethods
        private static ISellTransactionRequestProcessor CreateSellTransactionRequestProcessor()
        {
            var sellTransactionRequestProcessor = new SimpleSellTransactionRequestProcessor();
            return sellTransactionRequestProcessor;
        }
        #endregion
    }
}