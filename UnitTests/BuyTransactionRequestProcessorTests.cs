using Core.Interfaces;
using Infrastructure.TransactionRequests;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class BuyTransactionRequestProcessorTests
    {



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