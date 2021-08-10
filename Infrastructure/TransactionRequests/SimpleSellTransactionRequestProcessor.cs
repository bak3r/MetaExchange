using System.Collections.Generic;
using System.Linq;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.TransactionRequests
{
    public class SimpleSellTransactionRequestProcessor : ISellTransactionRequestProcessor
    {
        private readonly IBidCombinationSelector _bidCombinationSelector;
        private readonly IExchangeSelector _exchangeSelector;

        public SimpleSellTransactionRequestProcessor(IBidCombinationSelector bidCombinationSelector, IExchangeSelector exchangeSelector)
        {
            _bidCombinationSelector = bidCombinationSelector;
            _exchangeSelector = exchangeSelector;
        }
        public RequestProcessorResult ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges)
        {
            if (transactionRequest.TransactionAmount > 0)
            {
                if (cryptoExchanges.Any())
                {
                    var exchangesWithNonEmptyBidList = (from ce in cryptoExchanges
                        where ce.OrderBook.Bids.Count > 0
                        select ce).ToList();

                    if (exchangesWithNonEmptyBidList.Count > 0)
                    {
                        var tradableExchangesWithFilteredBidLists = new Dictionary<string, List<Bid>>();

                        foreach (var cryptoExchange in exchangesWithNonEmptyBidList)
                        {
                            var filteredBidListForCryptoExchange =
                                _bidCombinationSelector.PrepareListOfBidsToSatisfyTransactionAmount(
                                    transactionRequest.TransactionAmount, cryptoExchange.OrderBook.Bids);
                            if (filteredBidListForCryptoExchange != null)
                                tradableExchangesWithFilteredBidLists.Add(cryptoExchange.Name,
                                    filteredBidListForCryptoExchange);
                        }

                        var (selectedCryptoExchangeName, listOfNeededAsksToCompleteTransaction) =
                            _exchangeSelector.FindExchangeWithHighestPossibleBidTransactionValue(
                                tradableExchangesWithFilteredBidLists);

                        if (listOfNeededAsksToCompleteTransaction != null &&
                            listOfNeededAsksToCompleteTransaction.Any())
                        {
                            var hedgerTransactions = new List<HedgerTransaction>();

                            foreach (var singleBid in listOfNeededAsksToCompleteTransaction)
                            {
                                var hedgerTransaction = new HedgerTransaction()
                                {
                                    CryptoExchange = selectedCryptoExchangeName,
                                    Order = singleBid.Order
                                };
                                hedgerTransactions.Add(hedgerTransaction);
                            }

                            return new RequestProcessorResult()
                                { TransactionIsValid = true, HedgerTransactions = hedgerTransactions };
                        }

                        return new RequestProcessorResult()
                            { TransactionIsValid = false, ErrorMessage = "No crypto exchanges with bid high enough to satisfy the request exist." };
                    }

                    return new RequestProcessorResult()
                        { TransactionIsValid = false, ErrorMessage = "No crypto exchanges with non-empty bid list exist." };
                }
                else
                {
                    return new RequestProcessorResult()
                        { TransactionIsValid = false, ErrorMessage = "No crypto exchanges exist." };
                }
            }

            return new RequestProcessorResult()
                {TransactionIsValid = false, ErrorMessage = "Transaction amount must be larger than 0."};
        }
    }
}