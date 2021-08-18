using System.Collections.Generic;
using System.Linq;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.TransactionRequests
{
    /// <summary>
    /// Simple implementation of buy transaction processor. Main business logic for
    /// processing buy transactions is implemented here with a few details
    /// delegated to other services.
    /// </summary>
    public class SimpleBuyTransactionRequestProcessor : IBuyTransactionRequestProcessor
    {
        private readonly IExchangeSelector _exchangeSelector;
        private readonly ICombinationSelector<Ask> _combinationSelector;

        public SimpleBuyTransactionRequestProcessor(IExchangeSelector exchangeSelector, ICombinationSelector<Ask> combinationSelector)
        {
            _exchangeSelector = exchangeSelector;
            _combinationSelector = combinationSelector;
        }

        /// <summary>
        /// Entry point for each buy transaction processing.
        /// </summary>
        /// <param name="transactionRequest">DTO with requested transaction details</param>
        /// <param name="cryptoExchanges">List of all crypto exchanges</param>
        /// <returns>RequestProcessorResult DTO with hedger transactions and error status</returns>
        public RequestProcessorResult ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges)
        {
            if (transactionRequest.TransactionAmount > 0)
            {
                var cryptoExchangesWithEnoughBalance = (from e in cryptoExchanges
                                                        where e.BalanceBtc >= transactionRequest.TransactionAmount
                                                        select e).ToList();

                if (cryptoExchangesWithEnoughBalance.Any())
                {
                    var tradeableExchangesWithFilteredAskLists = new Dictionary<string, List<Ask>>();

                    foreach (var cryptoExchange in cryptoExchangesWithEnoughBalance)
                    {
                        var filteredAskListForCryptoExchange =
                            _combinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(
                                transactionRequest.TransactionAmount, cryptoExchange.OrderBook.Asks);
                        if (filteredAskListForCryptoExchange != null)
                            tradeableExchangesWithFilteredAskLists.Add(cryptoExchange.Name, filteredAskListForCryptoExchange);
                    }

                    var (selectedCryptoExchangeName, listOfNeededAsksToCompleteTransaction) =
                        _exchangeSelector.FindExchangeWithLowestPossibleAskTransactionCost(
                            tradeableExchangesWithFilteredAskLists);

                    if (listOfNeededAsksToCompleteTransaction != null && listOfNeededAsksToCompleteTransaction.Any())
                    {
                        var hedgerTransactions = new List<HedgerTransaction>();

                        foreach (var singleAsk in listOfNeededAsksToCompleteTransaction)
                        {
                            var hedgerTransaction = new HedgerTransaction
                            {
                                CryptoExchange = selectedCryptoExchangeName,
                                Order = singleAsk.Order
                            };
                            hedgerTransactions.Add(hedgerTransaction);
                        }

                        return new RequestProcessorResult()
                        { TransactionIsValid = true, HedgerTransactions = hedgerTransactions };
                    }

                    return new RequestProcessorResult()
                    {
                        TransactionIsValid = false,
                        ErrorMessage =
                            "List of Asks to complete transaction was null or empty or not enough asks exist to satisfy the requested amount."
                    };
                }
                return new RequestProcessorResult()
                { TransactionIsValid = false, ErrorMessage = "No crypto exchange with enough balance exist." };
            }
            return new RequestProcessorResult()
                { TransactionIsValid = false, ErrorMessage = "Transaction amount must be larger than 0." };

        }
    }
}