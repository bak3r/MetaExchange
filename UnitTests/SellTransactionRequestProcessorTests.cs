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
            var stubCryptoExchange2OrderBookBids = new List<Bid>()
                {new Bid() {Order = new Order() {Amount = 1m, Price = 1m}}};
            var stubExchange2Orderbook = new OrderBook { Bids = stubCryptoExchange2OrderBookBids };
            stubCryptoExchange2.OrderBook = stubExchange2Orderbook;

            stubCryptoExchanges.Add(stubCryptoExchange1);
            stubCryptoExchanges.Add(stubCryptoExchange2);

            
            BidCombinationSelectorMock
                .Setup(x => x.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequest.TransactionAmount, stubCryptoExchange2OrderBookBids))
                .Returns(stubCryptoExchange2OrderBookBids);

            var stubTradeableExchanegsWithFilteredBidLists
                = new Dictionary<string, List<Bid>>();
            stubTradeableExchanegsWithFilteredBidLists.Add(stubCryptoExchange2.Name, stubCryptoExchange2OrderBookBids);
            var mockedResponseFromExchangeSelector = (stubCryptoExchange2.Name, stubCryptoExchange2OrderBookBids);
            ExchangeSelectorMock.Setup(x =>
                    x.FindExchangeWithHighestPossibleBidTransactionValue(stubTradeableExchanegsWithFilteredBidLists))
                .Returns(mockedResponseFromExchangeSelector);

            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsTrue(result.TransactionIsValid);
            Assert.IsTrue(result.HedgerTransactions.Count == 1);
            Assert.IsTrue(result.HedgerTransactions[0].Order.Price == 1m);
            Assert.IsTrue(result.HedgerTransactions[0].Order.Amount == 1m);
        }

        [Test]
        public void T06_ProcessTransaction_MultipleExchangesNoneHasEnoughBidsToSatisfyTransactionAmount_TransactionIsInvalid()
        {
            var sellTransactionRequestProcessor = CreateSellTransactionRequestProcessor();
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = 100m, OrderType = OrderType.Sell };

            var stubCryptoExchanges = new List<CryptoExchange>();

            var stubCryptoExchange1 = new CryptoExchange() { Name = "StubCryptoExchange1" };
            var stubExchange1Orderbook = new OrderBook { Bids = new List<Bid>() { new Bid() { Order = new Order() { Amount = 0.5m, Price = 1m } } } };
            stubCryptoExchange1.OrderBook = stubExchange1Orderbook;

            var stubCryptoExchange2 = new CryptoExchange() { Name = "StubCryptoExchange2" };
            var stubCryptoExchange2OrderBookBids = new List<Bid>()
                {new Bid() {Order = new Order() {Amount = 1m, Price = 1m}}};
            var stubExchange2Orderbook = new OrderBook { Bids = stubCryptoExchange2OrderBookBids };
            stubCryptoExchange2.OrderBook = stubExchange2Orderbook;

            stubCryptoExchanges.Add(stubCryptoExchange1);
            stubCryptoExchanges.Add(stubCryptoExchange2);


            BidCombinationSelectorMock
                .Setup(x => x.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequest.TransactionAmount, stubCryptoExchange2OrderBookBids))
                .Returns(null as List<Bid>);

            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsFalse(result.TransactionIsValid);
        }

        [Test]
        public void T07_ProcessTransaction_OneExchangeMultipleAsksThatTogetherSatisfyTransactionAmount_HedgerTransactionsAreReturnedInResult()
        {
            var sellTransactionRequestProcessor = CreateSellTransactionRequestProcessor();
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = 30m, OrderType = OrderType.Sell };
            
            var stubBid1 = new Bid() {Order = new Order() {Amount = 10m, Price = 1m}};
            var stubBid2 = new Bid() {Order = new Order() {Amount = 10m, Price = 3m}};
            var stubBid3 = new Bid() {Order = new Order() {Amount = 10m, Price = 2m}};
            var stubBid4 = new Bid() {Order = new Order() {Amount = 10m, Price = 5m}};
            var stubCryptoExchange1bids = new List<Bid>() {stubBid1, stubBid2, stubBid3, stubBid4};
            var stubExchange1Orderbook = new OrderBook { Bids = stubCryptoExchange1bids };
            var stubCryptoExchange1 = new CryptoExchange
            {
                Name = "StubCryptoExchange1", OrderBook = stubExchange1Orderbook
            };
            var stubCryptoExchanges = new List<CryptoExchange> {stubCryptoExchange1};

            var mockedListOfBidsFromBidCombinationSelector = new List<Bid>() {stubBid4, stubBid2, stubBid3};
            BidCombinationSelectorMock
                .Setup(x => x.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequest.TransactionAmount, stubCryptoExchange1bids))
                .Returns(mockedListOfBidsFromBidCombinationSelector);

            var stubTradeableExchanegsWithFilteredBidLists
                = new Dictionary<string, List<Bid>>();
            stubTradeableExchanegsWithFilteredBidLists.Add(stubCryptoExchange1.Name, mockedListOfBidsFromBidCombinationSelector);
            var mockedResponseFromExchangeSelector = (stubCryptoExchange1.Name, mockedListOfBidsFromBidCombinationSelector);
            ExchangeSelectorMock.Setup(x =>
                    x.FindExchangeWithHighestPossibleBidTransactionValue(stubTradeableExchanegsWithFilteredBidLists))
                .Returns(mockedResponseFromExchangeSelector);

            var result = sellTransactionRequestProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsTrue(result.TransactionIsValid);
            Assert.IsTrue(result.HedgerTransactions.Count == 3);
            Assert.IsTrue(result.HedgerTransactions[0].Order.Price == 5m);
            Assert.IsTrue(result.HedgerTransactions[0].Order.Amount == 10m);
            Assert.IsTrue(result.HedgerTransactions[1].Order.Price == 3m);
            Assert.IsTrue(result.HedgerTransactions[1].Order.Amount == 10m);
            Assert.IsTrue(result.HedgerTransactions[2].Order.Price == 2m);
            Assert.IsTrue(result.HedgerTransactions[2].Order.Amount == 10m);
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