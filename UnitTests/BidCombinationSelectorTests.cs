using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;
using Infrastructure;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class BidCombinationSelectorTests
    {
        [Test]
        public void T01_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsZero_NullIsReturned()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();

            var result = bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(0m, new List<Bid>());

            Assert.IsNull(result);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void T02_PrepareListOfBidsToSatisfyTransactionAmount_OrderBookBidsListIsEmpty_NullIsReturned()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubEmptyListOfBids = new List<Bid>();
            var stubTransactionRequestAmount = 1m;

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubEmptyListOfBids);

            Assert.IsNull(result);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [TestCase(4)]
        [TestCase(5.2)]
        [TestCase(308.39372)]
        public void T03_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromOneAvailableBid_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 400m, Price = 1m, Type = stubBidType } };
            stubListOfBids.Add(stubBid1);

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(requestedBitcoinAmount,
                    stubListOfBids);

            Assert.IsNotEmpty(result);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [TestCase(4)]
        [TestCase(5.2)]
        [TestCase(308.39372)]
        public void T04_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromMultipleAvailableBids_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 400m, Price = 1m, Type = stubBidType } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 500m, Price = 1m, Type = stubBidType } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(requestedBitcoinAmount,
                    stubListOfBids);

            Assert.IsNotEmpty(result);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [TestCase(4)]
        [TestCase(5.2)]
        [TestCase(308.39372)]
        public void T05_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsHigherThanSumFromMultipleAvailableBids_NullIsReturned(decimal requestedBitcoinAmount)
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 1m, Price = 1m, Type = stubBidType } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 2m, Price = 1m, Type = stubBidType } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(requestedBitcoinAmount,
                    stubListOfBids);

            Assert.IsNull(result);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void
            T06_PrepareListOfBidsToSatisfyTransactionAmount_TwoBidsWithSufficientAmountExist_TheOneWithHigherPriceIsReturnedInList()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 1m, Price = 1m, Type = stubBidType } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 1m, Price = 2m, Type = stubBidType } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            var stubTransactionRequestAmount = 1m;

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result[0].Order.Price == 2m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void
            T07_PrepareListOfBidsToSatisfyTransactionAmount_TwoBidsWithSufficientAmountAndSamePriceExist_OnlyOneIsReturnedInList()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 1m, Price = 1m, Type = stubBidType } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 1m, Price = 1m, Type = stubBidType } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            var stubTransactionRequestAmount = 1m;

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 1m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void
            T08_PrepareListOfBidsToSatisfyTransactionAmount_BidWithHigherAmountAndHigherPriceExist_TheHigherPricedBidIsSelected()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m, Type = stubBidType } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 1m, Price = 1m, Type = stubBidType } };
            var stubBid3 = new Bid { Order = new Order() { Amount = 10m, Price = 3m, Type = stubBidType } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            stubListOfBids.Add(stubBid3);
            var stubTransactionRequestAmount = 1m;

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 3m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void
            T09_PrepareListOfBidsToSatisfyTransactionAmount_OneBidWithHigherAmountAndMultipleSmallerWithSamePriceExist_OneBidIsSelected()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m, Type = stubBidType } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m, Type = stubBidType } };
            var stubBid3 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m, Type = stubBidType } };
            var stubBid4 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m, Type = stubBidType } };
            var stubBid5 = new Bid { Order = new Order() { Amount = 10m, Price = 3m, Type = stubBidType } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            stubListOfBids.Add(stubBid3);
            stubListOfBids.Add(stubBid4);
            stubListOfBids.Add(stubBid5);
            var stubTransactionRequestAmount = 2m;

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 3m);
            Assert.IsTrue(result[0].Order.Amount == 2m);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(It.IsAny<string>()), Times.Never);
        }

        [TestCase(120)]
        [TestCase(150.2)]
        [TestCase(199.39372)]
        public void T10_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromMultipleAvailableBidsOnMultipleExchanges_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubCryptoExchange1name = "StubExchangeName1";
            var stubBid1 = new Bid { Order = new Order() { Amount = 100m, Price = 1m, Type = stubBidType }, CryptoExchangeName = stubCryptoExchange1name};
            var stubCryptoExchange2name = "StubExchangeName2";
            var stubBid2 = new Bid { Order = new Order() { Amount = 100m, Price = 2m, Type = stubBidType }, CryptoExchangeName = stubCryptoExchange2name };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);

            var result =
                bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(requestedBitcoinAmount,
                    stubListOfBids);

            Assert.IsNotEmpty(result);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(stubCryptoExchange1name), Times.Never);
            ExchangeBalanceTrackerMock.Verify(x => x.GetBalanceForExchange(stubCryptoExchange2name), Times.Never);
        }

        // TODO: prepare tests that like tests 4-9 but span multiple cryptoExchanges

        #region HelperMethods

        private ICombinationSelector<Bid> CreateBidCombinationSelector()
        {
            var bidCombinationSelector = new CombinationSelector<Bid>(ExchangeBalanceTrackerMock.Object);
            return bidCombinationSelector;
        }

        [SetUp]
        public void SetUp()
        {
            ExchangeBalanceTrackerMock = new Mock<IExchangeBalanceTracker>();
        }

        const string stubBidType = "Buy";
        public Mock<IExchangeBalanceTracker> ExchangeBalanceTrackerMock { get; set; }

        #endregion
    }
}