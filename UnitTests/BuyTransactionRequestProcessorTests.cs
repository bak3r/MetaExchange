using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;
using Infrastructure.TransactionRequests;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class BuyTransactionRequestProcessorTests
    {
        [Test]
        public void T01_ProcessTransaction_TransactionRequestAmountIsZero_ErrorMessageIsReturned()
        {
            var transactionProcessor = CreateSimpleBuyTransactionProcessor();
            var stubTransactionRequest = new TransactionRequest();
            stubTransactionRequest.TransactionAmount = 0m;
            
            var result = transactionProcessor.ProcessTransaction(stubTransactionRequest, new List<CryptoExchange>());

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("Transaction amount must be larger than 0.", result.ErrorMessage);
        }

        [Test]
        public void T02_ProcessRequest_NoExchangeWithEnoughBalanceExists_ErrorMessageIsReturned()
        {
            var transactionProcessor = CreateSimpleBuyTransactionProcessor();
            var stubTransactionRequest = new TransactionRequest();
            stubTransactionRequest.TransactionAmount = 1m;
            
            var result = transactionProcessor.ProcessTransaction(stubTransactionRequest, new List<CryptoExchange>());

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("No crypto exchange with enough balance exist.", result.ErrorMessage);
        }

        [Test]
        public void T03_ProcessRequest_ExchangeHasEnoughBalanceButNoAsksExist_ErrorMessageIsReturned()
        {
            var transactionProcessor = CreateSimpleBuyTransactionProcessor();
            var stubTransactionRequest = new TransactionRequest();
            stubTransactionRequest.TransactionAmount = 1m;
            var stubCryptoExchanges = new List<CryptoExchange>();
            var stubCryptoExchange = new CryptoExchange() {BalanceBtc = 1m, Name = "StubCryptoExchange", OrderBook = new OrderBook()};
            stubCryptoExchanges.Add(stubCryptoExchange);
            
            var result = transactionProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("List of Asks to complete transaction was null or empty or not enough asks exist to satisfy the requested amount.", result.ErrorMessage);
        }

        [Test]
        public void T04_ProcessRequest_ExchangeHasEnoughBalanceButNoAsksExistThatCanSatisfyRequestedAmount_ErrorMessageIsReturned()
        {
            var transactionProcessor = CreateSimpleBuyTransactionProcessor();
            var stubTransactionRequest = new TransactionRequest {TransactionAmount = 1m};
            var stubCryptoExchanges = new List<CryptoExchange>();
            var stubAskList = new List<Ask> {new Ask() {Order = new Order() {Amount = 0.1m, Price = 1m}}};
            var stubOrderBook = new OrderBook {Asks = stubAskList};
            var stubCryptoExchange = new CryptoExchange() { BalanceBtc = 1m, Name = "StubCryptoExchange", OrderBook = stubOrderBook };
            stubCryptoExchanges.Add(stubCryptoExchange);
            
            var result = transactionProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.IsFalse(result.TransactionIsValid);
            Assert.AreEqual("List of Asks to complete transaction was null or empty or not enough asks exist to satisfy the requested amount.", result.ErrorMessage);
        }

        [Test]
        public void T05_ProcessRequest_OneExchangeOneAskThatSatisfiesRequestedAmount_HedgerTransactionsAreReturnedInProcessorResult()
        {
            var transactionProcessor = CreateSimpleBuyTransactionProcessor();
            var transactionAmount = 1m;
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = transactionAmount };
            var stubCryptoExchanges = new List<CryptoExchange>();
            var stubAskList = new List<Ask> { new Ask() { Order = new Order() { Amount = 1m, Price = 1m } } };
            var stubOrderBook = new OrderBook { Asks = stubAskList };
            var stubCryptoExchangeName = "StubCryptoExchange";
            var stubCryptoExchange = new CryptoExchange() { BalanceBtc = 1m, Name = stubCryptoExchangeName, OrderBook = stubOrderBook };
            stubCryptoExchanges.Add(stubCryptoExchange);

            var stubAskList2 = new List<Ask> {new Ask() {Order = new Order() {Amount = 1m, Price = 1m}}};
            AskCombinationSelectorMock
                .Setup(x => x.PrepareListOfAsksToSatisfyTransactionAmount(transactionAmount, stubAskList))
                .Returns(stubAskList2);

            var stubDictionary = new Dictionary<string, List<Ask>> {{stubCryptoExchangeName, stubAskList2}};
            var stubExchangeWithLowestTransactionCost = (stubcryptoexchangeName: stubCryptoExchangeName, stubAskList2);
            ExchangeSelectorMock.Setup(x => x.FindExchangeWithLowestPossibleAskTransactionCost(stubDictionary))
                .Returns(stubExchangeWithLowestTransactionCost);
            
            var result = transactionProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.True(result.TransactionIsValid);
            Assert.True(result.HedgerTransactions.Count == 1);
        }

        
        [Test]
        public void
            T06_ProcessRequest_OneExchangeMultipleAsksThatTogetherSatisfyRequestedAmount_HedgerTransactionsAreReturnedInProcessorResult()
        {
            var transactionProcessor = CreateSimpleBuyTransactionProcessor();
            var transactionAmount = 2m;
            var stubTransactionRequest = new TransactionRequest {TransactionAmount = transactionAmount};
            var stubCryptoExchanges = new List<CryptoExchange>();
            var stubAsk1 = new Ask() {Order = new Order() {Amount = 1m, Price = 1m}};
            var stubAsk2 = new Ask() {Order = new Order() {Amount = 1m, Price = 2m}};
            var stubAsk3 = new Ask() {Order = new Order() {Amount = 10m, Price = 3m}};
            var stubAskList = new List<Ask> {stubAsk1, stubAsk2, stubAsk3};

        var stubOrderBook = new OrderBook { Asks = stubAskList };
            var stubCryptoExchangeName = "StubCryptoExchange";
            var stubCryptoExchange = new CryptoExchange() { BalanceBtc = 2m, Name = stubCryptoExchangeName, OrderBook = stubOrderBook };
            stubCryptoExchanges.Add(stubCryptoExchange);

            var mockReturnedAskList = new List<Ask> { stubAsk1, stubAsk2 };
            AskCombinationSelectorMock
                .Setup(x => x.PrepareListOfAsksToSatisfyTransactionAmount(transactionAmount, stubAskList))
                .Returns(mockReturnedAskList);

            var stubDictionary = new Dictionary<string, List<Ask>> { { stubCryptoExchangeName, mockReturnedAskList } };
            var mockReturnedExchangeWithLowestTransactionCost = (stubcryptoexchangeName: stubCryptoExchangeName, stubAskList2: mockReturnedAskList);
            ExchangeSelectorMock.Setup(x => x.FindExchangeWithLowestPossibleAskTransactionCost(stubDictionary))
                .Returns(mockReturnedExchangeWithLowestTransactionCost);

            var result = transactionProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.True(result.TransactionIsValid);
            Assert.True(result.HedgerTransactions.Count == 2);
        }

        // multiple crypto exchange, multiple asks that satisfies request, transaction valid, list of hedger transactions

        [Test]
        public void
            T07_ProcessRequest_MultipleExchangesMultipleAsksThatTogetherSatisfyRequestedAmount_HedgerTransactionsAreReturnedInProcessorResult()
        {
            var transactionProcessor = CreateSimpleBuyTransactionProcessor();
            var transactionAmount = 2m;
            var stubTransactionRequest = new TransactionRequest { TransactionAmount = transactionAmount };

            var stubCryptoExchanges = new List<CryptoExchange>();
            var stubExchange1Ask1 = new Ask() { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubExchange1Ask2 = new Ask() { Order = new Order() { Amount = 1m, Price = 2m } };
            var stubExchange1Ask3 = new Ask() { Order = new Order() { Amount = 10m, Price = 3m } };
            var stubExchange2Ask1 = new Ask() { Order = new Order() { Amount = 0.5m, Price = 1m } };
            var stubExchange2Ask2 = new Ask() { Order = new Order() { Amount = 3m, Price = 3m } };
            var stubExchange2Ask3 = new Ask() { Order = new Order() { Amount = 5m, Price = 1 } };
            var stubAskList1 = new List<Ask> { stubExchange1Ask1, stubExchange1Ask2, stubExchange1Ask3 };
            var stubAskList2 = new List<Ask> { stubExchange2Ask1, stubExchange2Ask2, stubExchange2Ask3 };

            var stubOrderBook1 = new OrderBook { Asks = stubAskList1 };
            var stubOrderBook2 = new OrderBook { Asks = stubAskList2 };
            var stubCryptoExchangeName1 = "StubCryptoExchange1";
            var stubCryptoExchangeName2 = "StubCryptoExchange2";
            var stubCryptoExchange1 = new CryptoExchange() { BalanceBtc = 2m, Name = stubCryptoExchangeName1, OrderBook = stubOrderBook1 };
            var stubCryptoExchange2 = new CryptoExchange() { BalanceBtc = 2m, Name = stubCryptoExchangeName2, OrderBook = stubOrderBook2 };
            stubCryptoExchanges.Add(stubCryptoExchange1);
            stubCryptoExchanges.Add(stubCryptoExchange2);

            var mockReturnedAskList1 = new List<Ask> { stubExchange1Ask1, stubExchange1Ask2 };
            var mockReturnedAskList2 = new List<Ask> { stubExchange2Ask1, new Ask(){Order = new Order(){Amount = 1.5m, Price = 1m}} };
            AskCombinationSelectorMock
                .Setup(x => x.PrepareListOfAsksToSatisfyTransactionAmount(transactionAmount, stubAskList1))
                .Returns(mockReturnedAskList1);
            AskCombinationSelectorMock
                .Setup(x => x.PrepareListOfAsksToSatisfyTransactionAmount(transactionAmount, stubAskList2))
                .Returns(mockReturnedAskList2);

            var stubDictionary = new Dictionary<string, List<Ask>>
                {{stubCryptoExchangeName1, mockReturnedAskList1}, {stubCryptoExchangeName2, mockReturnedAskList2}};

            var mockReturnedExchangeWithLowestTransactionCost = (stubcryptoexchangeName: stubCryptoExchangeName2,
                stubAskList2: mockReturnedAskList2);
            ExchangeSelectorMock.Setup(x => x.FindExchangeWithLowestPossibleAskTransactionCost(stubDictionary))
                .Returns(mockReturnedExchangeWithLowestTransactionCost);

            var result = transactionProcessor.ProcessTransaction(stubTransactionRequest, stubCryptoExchanges);

            Assert.True(result.TransactionIsValid);
            Assert.True(result.HedgerTransactions.Count == 2);
            Assert.IsTrue(
                result.HedgerTransactions[0].Order.Amount == 0.5m && result.HedgerTransactions[0].Order.Price == 1m,
                "First transaction should have been the one with Amount=0.5 but was {0} and Price=1 but was {1}",
                result.HedgerTransactions[0].Order.Amount, result.HedgerTransactions[0].Order.Price);
            Assert.IsTrue(
                result.HedgerTransactions[1].Order.Amount == 1.5m && result.HedgerTransactions[1].Order.Price == 1m,
                "Second transaction should have been the one with Amount=1.5 but was {0} and Price=1 but was {1}",
                result.HedgerTransactions[1].Order.Amount, result.HedgerTransactions[1].Order.Price);
        }


        private Mock<IExchangeSelector> ExchangeSelectorMock { get; set; }
        private Mock<IAskCombinationSelector> AskCombinationSelectorMock { get; set; }

        #region HelperMethods
        [SetUp]
        public void SetUp()
        {
            ExchangeSelectorMock = new Mock<IExchangeSelector>();
            AskCombinationSelectorMock = new Mock<IAskCombinationSelector>();
        }
        public SimpleBuyTransactionRequestProcessor CreateSimpleBuyTransactionProcessor()
        {
            var simpleBuyTransactionRequestProcessor =
                new SimpleBuyTransactionRequestProcessor(ExchangeSelectorMock.Object,
                    AskCombinationSelectorMock.Object);
            return simpleBuyTransactionRequestProcessor;
        }
        #endregion

    }
}