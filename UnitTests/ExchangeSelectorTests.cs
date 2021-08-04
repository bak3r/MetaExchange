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

            var stubMultipleExchangesWithAsks = new Dictionary<string, List<Ask>>();
            var result = simpleExchangeLocator.FindExchangeWithLowestPossibleAskTransactionCost(stubMultipleExchangesWithAsks);

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

            var exchangeName1 = "StubExchange1";
            var exchange1askList = new List<Ask>();
            var ask1 = new Ask();
            ask1.Order = new Order() { Amount = 1m, Price = 1m };
            exchange1askList.Add(ask1);

            var exchangeName2 = "StubExchange2";
            var exchange2askList = new List<Ask>();
            var ask2 = new Ask();
            ask2.Order = new Order() { Amount = 1m, Price = 2m };
            exchange2askList.Add(ask2);

            stubMultipleExchangesWithAsks.Add(exchangeName1, exchange1askList);
            stubMultipleExchangesWithAsks.Add(exchangeName2, exchange2askList);

            var result = simpleExchangeLocator.FindExchangeWithLowestPossibleAskTransactionCost(stubMultipleExchangesWithAsks);

            Assert.AreEqual(result.Item1, exchangeName1);
            Assert.Contains(ask1, result.Item2);
        }

        [Test]
        public void T04_FindExchangeWithLowestPossibleAskTransactionCost_TwoExchangesSameAskSumsExist_OnlyOneExchangeIsReturned()
        {
            var simpleExchangeLocator = new SimpleExchangeSelector();

            var stubMultipleExchangesWithAsks = new Dictionary<string, List<Ask>>();

            var exchangeName1 = "StubExchange1";
            var exchange1askList = new List<Ask>();
            var ask1 = new Ask();
            ask1.Order = new Order() { Amount = 1m, Price = 1m, Id = "1" };
            exchange1askList.Add(ask1);

            var exchangeName2 = "StubExchange2";
            var exchange2askList = new List<Ask>();
            var ask2 = new Ask();
            ask2.Order = new Order() { Amount = 1m, Price = 1m, Id = "2" };
            exchange2askList.Add(ask2);

            stubMultipleExchangesWithAsks.Add(exchangeName1, exchange1askList);
            stubMultipleExchangesWithAsks.Add(exchangeName2, exchange2askList);

            var result = simpleExchangeLocator.FindExchangeWithLowestPossibleAskTransactionCost(stubMultipleExchangesWithAsks);

            Assert.NotNull(result.Item1);
            Assert.NotNull(result.Item2);
            Assert.True(result.Item2.Count == 1);
        }
    }
}