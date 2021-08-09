using System.Collections.Generic;
using System.Linq;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.TransactionRequests
{
    public class SimpleBuyTransactionRequestProcessor : IBuyTransactionRequestProcessor
    {
        private readonly IExchangeSelector _exchangeSelector;
        private readonly IAskCombinationSelector _askCombinationSelector;

        public SimpleBuyTransactionRequestProcessor(IExchangeSelector exchangeSelector, IAskCombinationSelector askCombinationSelector)
        {
            _exchangeSelector = exchangeSelector;
            _askCombinationSelector = askCombinationSelector;
        }
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
                        var filteredAskListForCryptoExchange = _askCombinationSelector.PrepareListOfAsksToSatisfyTransactionAmount(
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