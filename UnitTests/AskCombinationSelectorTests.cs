using System.Collections.Generic;
using Core.Implementations.DTOs;
using Infrastructure.Asks;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class AskCombinationSelectorTests
    {
        [Test]
        public void T01_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsZero_NullIsReturned()
        {
            var simpleAskCombinationSelector = CreateSimpleAskCombinationSelector();

            var result = simpleAskCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(0, new List<Ask>());

            Assert.IsNull(result);
        }

        [Test]
        public void T02_PrepareListOfAsksToSatisfyTransactionAmount_OrderBookAsksListIsEmpty_NullIsReturned()
        {
            var simpleAskCombinationSelector = CreateSimpleAskCombinationSelector();

            var result = simpleAskCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(1, new List<Ask>());

            Assert.IsNull(result);
        }

        [TestCase(3)]
        [TestCase(5.6)]
        [TestCase(8.39372)]
        public void T03_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromOneAvailableAsk_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var simpleAskCombinationSelector = CreateSimpleAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 15m, Price = 1m } };
            stubListOfAsks.Add(stubAsk1);
            var result = simpleAskCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(requestedBitcoinAmount, stubListOfAsks);

            Assert.IsNotEmpty(result);
        }

        [TestCase(1)]
        [TestCase(1.5)]
        [TestCase(2.9)]
        public void T04_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromMultipleAsks_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var simpleAskCombinationSelector = CreateSimpleAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 2m, Price = 1m } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var result = simpleAskCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(requestedBitcoinAmount, stubListOfAsks);

            Assert.IsNotEmpty(result);
        }

        [TestCase(100)]
        [TestCase(10.5)]
        [TestCase(1111.9999)]
        public void T05_PrepareListOfAsksToSatisfyTransactionAmount_RequestedBitcoinAmountIsHigherThanSumFromMultipleAsks_NullIsReturned(decimal requestedBitcoinAmount)
        {
            var simpleAskCombinationSelector = CreateSimpleAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 2m, Price = 1m } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var result = simpleAskCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(requestedBitcoinAmount, stubListOfAsks);

            Assert.IsNull(result);
        }

        [Test]
        public void T06_PrepareListOfAsksToSatisfyTransactionAmount_TwoAsksWithSufficientAmountExist_TheOneWithLowerPriceIsReturnedInList()
        {
            var simpleAskCombinationSelector = CreateSimpleAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 1m, Price = 2m } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var result = simpleAskCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(1m, stubListOfAsks);

            Assert.IsTrue(result[0].Order.Price == 1m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        [Test]
        public void T07_PrepareListOfAsksToSatisfyTransactionAmount_TwoAsksWithSufficientAmountAndSamePriceExist_OnlyOneIsReturnedInList()
        {
            var simpleAskCombinationSelector = CreateSimpleAskCombinationSelector();
            var stubListOfAsks = new List<Ask>();
            var stubAsk1 = new Ask { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubAsk2 = new Ask { Order = new Order() { Amount = 1m, Price = 1m } };
            stubListOfAsks.Add(stubAsk1);
            stubListOfAsks.Add(stubAsk2);
            var result = simpleAskCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(1m, stubListOfAsks);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 1m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        #region HelperMethods

        private SimpleAskCombinationSelector CreateSimpleAskCombinationSelector()
        {
            var simpleAskCombinationSelector = new SimpleAskCombinationSelector();
            return simpleAskCombinationSelector;
        }

        #endregion
    }
}