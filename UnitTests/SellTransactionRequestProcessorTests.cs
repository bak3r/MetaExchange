using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Implementations.Enums;
using Core.Interfaces;
using Infrastructure.TransactionRequests;
using Moq;
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
        [Ignore("Need to mock bidCombinationSelector and exchangeSelector first")]
        [Test]
        public void T05_ProcessTransaction_TwoExchangesExistsOnlyOneWithEnoughBidsToSatisfyTheRequest_TransactionIsValid()
        {
            var sellTransactionRequestProcessor = CreateSellTransactionRequestProcessor();
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = 1m, OrderType = OrderType.Sell };

            var stubCryptoExchanges = new List<CryptoExchange>();
            
            var stubCryptoExchange1 = new CryptoExchange() {Name = "StubCryptoExchange1"};
            var stubExchange1Orderbook = new OrderBook {Bids = new List<Bid>() {new Bid() {Order = new Order() {Amount = 0.5m, Price = 1m}}}};
            stubCryptoExchange1.OrderBook = stubExchange1Orderbook;

            var stubCryptoExchange2 = new CryptoExchange() {Name = "StubCryptoExchange2"};
            var stubExchange2Orderbook = new OrderBook { Bids = new List<Bid>() { new Bid() { Order = new Order() { Amount = 1m, Price = 1m} } } };
            stubCryptoExchange2.OrderBook = stubExchange2Orderbook;

            stubCryptoExchanges.Add(stubCryptoExchange1);
            stubCryptoExchanges.Add(stubCryptoExchange2);

            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsTrue(result.TransactionIsValid);
        }

        private Mock<IExchangeSelector> ExchangeSelectorMock { get; set; }
        private Mock<IBidCombinationSelector> BidCombinationSelectorMock { get; set; }

        #region HelperMethods
        [SetUp]
        public void SetUp()
        {
            ExchangeSelectorMock = new Mock<IExchangeSelector>();
            BidCombinationSelectorMock = new Mock<IBidCombinationSelector>();
        }
        private ISellTransactionRequestProcessor CreateSellTransactionRequestProcessor()
        {
            var sellTransactionRequestProcessor =
                new SimpleSellTransactionRequestProcessor(BidCombinationSelectorMock.Object,
                    ExchangeSelectorMock.Object);
            return sellTransactionRequestProcessor;
        }
        #endregion
    }
}