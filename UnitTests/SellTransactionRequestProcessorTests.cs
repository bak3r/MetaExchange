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

        #region HelperMethods
        private static ISellTransactionRequestProcessor CreateSellTransactionRequestProcessor()
        {
            var sellTransactionRequestProcessor = new SimpleSellTransactionRequestProcessor();
            return sellTransactionRequestProcessor;
        }
        #endregion
    }
}