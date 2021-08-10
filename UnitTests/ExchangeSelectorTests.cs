using System.Collections.Generic;
using Core.Implementations.DTOs;
using Infrastructure.CryptoExchanges;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ExchangeSelectorTests
    {
        [Test]
        public void T01_FindExchangeWithLowestPossibleAskTransactionCost_NoExchangesExist_NullIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubEmptyAskDictionary = new Dictionary<string, List<Ask>>();
            var result = simpleExchangeLocator.FindExchangeWithLowestPossibleAskTransactionCost(stubEmptyAskDictionary);

            Assert.IsNull(result.Item1);
            Assert.IsNull(result.Item2);
        }

        [Test]
        public void T02_FindExchangeWithLowestPossibleAskTransactionCost_OnlyExchangeWithEmptyListOfAsksExist_NullIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubMultipleExchangesWithAsks = new Dictionary<string, List<Ask>>();
            stubMultipleExchangesWithAsks.Add("StubExchangeName", new List<Ask>());
            var result = simpleExchangeLocator.FindExchangeWithLowestPossibleAskTransactionCost(stubMultipleExchangesWithAsks);

            Assert.IsNull(result.Item1);
            Assert.IsNull(result.Item2);
        }

        [Test]
        public void T03_FindExchangeWithLowestPossibleAskTransactionCost_TwoExchangesDifferentAskSumsExist_ExchangeWithLowerAskSumIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubMultipleExchangesWithAsks = new Dictionary<string, List<Ask>>();

            const string exchangeName1 = "StubExchange1";
            var exchange1AskList = new List<Ask>();
            var ask1 = new Ask {Order = new Order() {Amount = 1m, Price = 1m}};
            exchange1AskList.Add(ask1);

            const string exchangeName2 = "StubExchange2";
            var exchange2AskList = new List<Ask>();
            var ask2 = new Ask {Order = new Order() {Amount = 1m, Price = 2m}};
            exchange2AskList.Add(ask2);

            stubMultipleExchangesWithAsks.Add(exchangeName1, exchange1AskList);
            stubMultipleExchangesWithAsks.Add(exchangeName2, exchange2AskList);

            var result = simpleExchangeLocator.FindExchangeWithLowestPossibleAskTransactionCost(stubMultipleExchangesWithAsks);

            Assert.AreEqual(exchangeName1, result.Item1);
            Assert.Contains(ask1, result.Item2);
        }

        [Test]
        public void T04_FindExchangeWithLowestPossibleAskTransactionCost_TwoExchangesSameAskSumsExist_OnlyOneExchangeIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubMultipleExchangesWithAsks = new Dictionary<string, List<Ask>>();

            const string exchangeName1 = "StubExchange1";
            var exchange1AskList = new List<Ask>();
            var ask1 = new Ask {Order = new Order() {Amount = 1m, Price = 1m, Id = "1"}};
            exchange1AskList.Add(ask1);

            const string exchangeName2 = "StubExchange2";
            var exchange2AskList = new List<Ask>();
            var ask2 = new Ask {Order = new Order() {Amount = 1m, Price = 1m, Id = "2"}};
            exchange2AskList.Add(ask2);

            stubMultipleExchangesWithAsks.Add(exchangeName1, exchange1AskList);
            stubMultipleExchangesWithAsks.Add(exchangeName2, exchange2AskList);

            var result = simpleExchangeLocator.FindExchangeWithLowestPossibleAskTransactionCost(stubMultipleExchangesWithAsks);

            Assert.NotNull(result.Item1);
            Assert.NotNull(result.Item2);
            Assert.True(result.Item2.Count == 1);
        }

        [Test]
        public void T05_FindExchangeWithHighestPossibleBidTransactionValue_NoExchangesExist_NullIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubEmptyExchangeDictionary = new Dictionary<string, List<Bid>>();
            var result = simpleExchangeLocator.FindExchangeWithHighestPossibleBidTransactionValue(stubEmptyExchangeDictionary);

            Assert.IsNull(result.Item1);
            Assert.IsNull(result.Item2);
        }

        [Test]
        public void T06_FindExchangeWithHighestPossibleBidTransactionValue_OnlyExchangeWithEmptyBidListExist_NullIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubEmptyExchangeDictionary = new Dictionary<string, List<Bid>>();
            stubEmptyExchangeDictionary.Add("StubExchangeName", new List<Bid>());
            var result = simpleExchangeLocator.FindExchangeWithHighestPossibleBidTransactionValue(stubEmptyExchangeDictionary);

            Assert.IsNull(result.Item1);
            Assert.IsNull(result.Item2);
        }

        [Test]
        public void T07_FindExchangeWithHighestPossibleBidTransactionValue_MultipleExchangesDifferentBidSumsExist_ExchangeWithHighestBidSumIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubExchange1Bid1 = new Bid {Order = new Order() {Amount = 1m, Price = 1m}};
            var stubExchange1Bid2 = new Bid {Order = new Order() {Amount = 2m, Price = 2m}};
            var stubCryptoExchange1 = new CryptoExchange()
                {Name = "StubExchange1", OrderBook = new OrderBook() {Bids = {stubExchange1Bid1, stubExchange1Bid2}}};

            var stubExchange2Bid1 = new Bid {Order = new Order() {Amount = 1m, Price = 3m}};
            var stubBid2 = new Bid() {Order = new Order() {Amount = 2m, Price = 4m}};
            var stubExchange2Bid2 = stubBid2;
            var stubCryptoExchange2 = new CryptoExchange()
                {Name = "StubExchange2", OrderBook = new OrderBook() {Bids = {stubExchange2Bid1, stubExchange2Bid2}}};

            var stubExchangeDictionary = new Dictionary<string, List<Bid>>
            {
                {stubCryptoExchange1.Name, stubCryptoExchange1.OrderBook.Bids},
                {stubCryptoExchange2.Name, stubCryptoExchange2.OrderBook.Bids}
            };

            var result = simpleExchangeLocator.FindExchangeWithHighestPossibleBidTransactionValue(stubExchangeDictionary);

            Assert.AreEqual(stubCryptoExchange2.Name, result.Item1);
            Assert.Contains(stubBid2, result.Item2);
        }

        [Test]
        public void T08_FindExchangeWithHighestPossibleBidTransactionValue_MultipleExchangesWithSameBidSumsExist_OnlyOneExchangeIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubExchange1Bid1 = new Bid { Order = new Order() { Amount = 1m, Price = 2m } };
            var stubCryptoExchange1 = new CryptoExchange()
                { Name = "StubExchange1", OrderBook = new OrderBook() { Bids = { stubExchange1Bid1 } } };

            var stubExchange2Bid1 = new Bid { Order = new Order() { Amount = 1m, Price = 2m } };
            
            var stubCryptoExchange2 = new CryptoExchange()
                { Name = "StubExchange2", OrderBook = new OrderBook() { Bids = { stubExchange2Bid1 } } };

            var stubExchangeDictionary = new Dictionary<string, List<Bid>>
            {
                {stubCryptoExchange1.Name, stubCryptoExchange1.OrderBook.Bids},
                {stubCryptoExchange2.Name, stubCryptoExchange2.OrderBook.Bids}
            };

            var result = simpleExchangeLocator.FindExchangeWithHighestPossibleBidTransactionValue(stubExchangeDictionary);

            Assert.NotNull(result.Item1);
            Assert.NotNull(result.Item2);
            Assert.True(result.Item2.Count == 1);
            Assert.AreEqual(1, result.Item2[0].Order.Amount);
            Assert.AreEqual(2, result.Item2[0].Order.Price);
        }
    }
}