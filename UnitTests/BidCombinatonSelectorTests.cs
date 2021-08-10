using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;
using Infrastructure.Bids;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class BidCombinatonSelectorTests
    {
        [Test]
        public void T01_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsZero_NullIsReturned()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();

            var result = bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(0m, new List<Bid>());

            Assert.IsNull(result);
        }

        [Test]
        public void T02_PrepareListOfBidsToSatisfyTransactionAmount_OrderBookBidsListIsEmpty_NullIsReturned()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();

            var stubEmptyListOfBids = new List<Bid>();
            var stubTransactionRequestAmount = 1m;
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubEmptyListOfBids);

            Assert.IsNull(result);
        }

        [TestCase(4)]
        [TestCase(5.2)]
        [TestCase(308.39372)]
        public void T03_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromOneAvailableBid_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 400m, Price = 1m } };
            stubListOfBids.Add(stubBid1);
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(requestedBitcoinAmount,
                    stubListOfBids);

            Assert.IsNotEmpty(result);
        }

        [TestCase(4)]
        [TestCase(5.2)]
        [TestCase(308.39372)]
        public void T04_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsLowerThanSumFromMultipleAvailableBids_NonEmptyListIsReturned(decimal requestedBitcoinAmount)
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 400m, Price = 1m } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 500m, Price = 1m } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(requestedBitcoinAmount,
                    stubListOfBids);

            Assert.IsNotEmpty(result);
        }

        [TestCase(4)]
        [TestCase(5.2)]
        [TestCase(308.39372)]
        public void T05_PrepareListOfBidsToSatisfyTransactionAmount_RequestedBitcoinAmountIsHigherThanSumFromMultipleAvailableBids_NullIsReturned(decimal requestedBitcoinAmount)
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 2m, Price = 1m } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(requestedBitcoinAmount,
                    stubListOfBids);

            Assert.IsNull(result);
        }

        [Test]
        public void
            T06_PrepareListOfBidsToSatisfyTransactionAmount_TwoBidsWithSufficientAmountExist_TheOneWithHigherPriceIsReturnedInList()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 1m, Price = 2m } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            var stubTransactionRequestAmount = 1m;
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result[0].Order.Price == 2m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        [Test]
        public void
            T07_PrepareListOfBidsToSatisfyTransactionAmount_TwoBidsWithSufficientAmountAndSamePriceExist_OnlyOneIsReturnedInList()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 1m, Price = 1m } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            var stubTransactionRequestAmount = 1m;
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 1m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        [Test]
        public void
            T08_PrepareListOfBidsToSatisfyTransactionAmount_BidWithHigherAmountAndHigherPriceExist_TheHigherPricedBidIsSelected()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 1m, Price = 1m } };
            var stubBid3 = new Bid { Order = new Order() { Amount = 10m, Price = 3m } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            stubListOfBids.Add(stubBid3);
            var stubTransactionRequestAmount = 1m;
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 3m);
            Assert.IsTrue(result[0].Order.Amount == 1m);
        }

        [Test]
        public void
            T09_PrepareListOfBidsToSatisfyTransactionAmount_OneBidWithHigherAmountAndMultipleSmallerWithSamePriceExist_OneBidIsSelected()
        {
            var bidCombinationSelector = CreateBidCombinationSelector();
            var stubListOfBids = new List<Bid>();
            var stubBid1 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m } };
            var stubBid2 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m } };
            var stubBid3 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m } };
            var stubBid4 = new Bid { Order = new Order() { Amount = 0.5m, Price = 3m } };
            var stubBid5 = new Bid { Order = new Order() { Amount = 10m, Price = 3m } };
            stubListOfBids.Add(stubBid1);
            stubListOfBids.Add(stubBid2);
            stubListOfBids.Add(stubBid3);
            stubListOfBids.Add(stubBid4);
            stubListOfBids.Add(stubBid5);
            var stubTransactionRequestAmount = 2m;
            var result =
                bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(stubTransactionRequestAmount,
                    stubListOfBids);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Order.Price == 3m);
            Assert.IsTrue(result[0].Order.Amount == 2m);
        }

        #region HelperMethods

        private IBidCombinationSelector CreateBidCombinationSelector()
        {
            var bidCombinationSelector = new SimpleBidCombinationSelector();
            return bidCombinationSelector;
        }

        

        #endregion
    }
}