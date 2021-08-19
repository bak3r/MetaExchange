using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;
using Infrastructure;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class AskCombinationSelectorTests
    {
        [Test]
        public void T01_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsZero_NullIsReturned()
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(0, new List<Ask>());
            

            Assert.IsNull(result);
        }

        [Test]
        public void T02_PrepareListOfAsksToSatisfyTransactionAmount_OrderBookAsksListIsEmpty_NullIsReturned()
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(1, new List<Ask>());

            Assert.IsNull(result);
        }

        [TestCase(3)]
        [TestCase(5.6)]
        [TestCase(8.39372)]
        public void T03_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromOneAvailableAsk_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 15m, Price = 1m, Type = stubAskType} };
            stubListOfAsks.Add(stubAsk1);
            var stubHigherBalanceThanRequestedAmount = 500m;
            ExchangeBalanceTrackerMock.Setup(x => x.GetBalanceForExchange(It.IsAny<string>())).Returns(stubHigherBalanceThanRequestedAmount);

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(requestedBitcoinAmount, stubListOfAsks);

            Assert.IsNotEmpty(result);
        }

        [TestCase(1)]
        [TestCase(1.5)]
        [TestCase(2.9)]
        public void T04_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromMultipleAsks_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m, Type = stubAskType } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 2m, Price = 1m, Type = stubAskType } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var stubHigherBalanceThanRequestedAmount = 500m;
            ExchangeBalanceTrackerMock.Setup(x => x.GetBalanceForExchange(It.IsAny<string>())).Returns(stubHigherBalanceThanRequestedAmount);

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(requestedBitcoinAmount, stubListOfAsks);

            Assert.IsNotEmpty(result);
        }

        [TestCase(100)]
        [TestCase(10.5)]
        [TestCase(1111.9999)]
        public void T05_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsHigherThanSumFromMultipleAsks_NullIsReturned(decimal requestedBitcoinAmount)
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m, Type = stubAskType } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 2m, Price = 1m, Type = stubAskType } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var stubHigherBalanceThanRequestedAmount = 500m;
            ExchangeBalanceTrackerMock.Setup(x => x.GetBalanceForExchange(It.IsAny<string>())).Returns(stubHigherBalanceThanRequestedAmount);

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(requestedBitcoinAmount, stubListOfAsks);

            Assert.IsNull(result);
        }

        [Test]
        public void T06_PrepareListOfAsksToSatisfyTransactionAmount_TwoAsksWithSufficientAmountExist_TheOneWithLowerPriceIsReturnedInList()
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m, Type = stubAskType } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 1m, Price = 2m, Type = stubAskType } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var stubHigherBalanceThanRequestedAmount = 500m;
            ExchangeBalanceTrackerMock.Setup(x => x.GetBalanceForExchange(It.IsAny<string>())).Returns(stubHigherBalanceThanRequestedAmount);

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(1m, stubListOfAsks);

            Assert.IsTrue(result[0].Order.Price == 1m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        [Test]
        public void T07_PrepareListOfAsksToSatisfyTransactionAmount_TwoAsksWithSufficientAmountAndSamePriceExist_OnlyOneIsReturnedInList()
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m, Type = stubAskType } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 1m, Price = 1m, Type = stubAskType } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var stubHigherBalanceThanRequestedAmount = 500m;
            ExchangeBalanceTrackerMock.Setup(x => x.GetBalanceForExchange(It.IsAny<string>())).Returns(stubHigherBalanceThanRequestedAmount);

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(1m, stubListOfAsks);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 1m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        [Test]
        public void T08_PrepareListOfAsksToSatisfyTransactionAmount_HigherAmountWithLowerPriceAskExists_TheLowerPricedAskIsSelected()
        {
            var simpleAskCombinationSelector = CreateAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 0.5m, Price = 1m, Type = stubAskType } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 3m, Price = 3m, Type = stubAskType } };
            var stubAsk3 = new Ask { Order = new Order() { Amount = 10m, Price = 0.5m, Type = stubAskType } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            stubListOfAsks.Add(stubAsk3);
            var stubHigherBalanceThanRequestedAmount = 500m;
            ExchangeBalanceTrackerMock.Setup(x => x.GetBalanceForExchange(It.IsAny<string>())).Returns(stubHigherBalanceThanRequestedAmount);

            var result = simpleAskCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(1m, stubListOfAsks);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 0.5m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        // TODO: prepare tests for when asks are spread amongst multiple cryptoExchanges
        // TODO: prepare tests involving exchange balance not being sufficient


        #region HelperMethods

        private ICombinationSelector<Ask> CreateAskCombinationSelector()
        {
            var askCombinationSelector = new CombinationSelector<Ask>(ExchangeBalanceTrackerMock.Object);
            return askCombinationSelector;
        }

        [SetUp]
        public void SetUp()
        {
            ExchangeBalanceTrackerMock = new Mock<IExchangeBalanceTracker>();
        }

        const string stubAskType = "Sell";
        public Mock<IExchangeBalanceTracker> ExchangeBalanceTrackerMock { get; set; }

        #endregion
    }
}