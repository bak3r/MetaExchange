using System.Collections.Generic;
using System.Linq;
using Core.Implementations.DTOs;
using Core.Implementations.Enums;
using Core.Interfaces;

namespace Core.Implementations
{
    /// <summary>
    /// Entry point for processing transaction requests.
    /// </summary>
    public class TransactionRequestProcessor : ITransactionRequestProcessor
    {
        private readonly ICombinationSelector<Bid> _bidCombinationSelector;
        private readonly ICombinationSelector<Ask> _askCombinationSelector;

        public TransactionRequestProcessor(ICombinationSelector<Bid> bidCombinationSelector, ICombinationSelector<Ask> askCombinationSelector)
        {
            _bidCombinationSelector = bidCombinationSelector;
            _askCombinationSelector = askCombinationSelector;
        }

        /// <summary>
        /// Handles flow for preparing list of bids/asks and producing Hedger transactions for specified transaction request
        /// </summary>
        /// <param name="transactionRequest">Transaction request that needs to be processed</param>
        /// <param name="cryptoExchanges">List of crypto exchanges</param>
        /// <returns></returns>
        public RequestProcessorResult ProcessTransaction(TransactionRequest transactionRequest, List<CryptoExchange> cryptoExchanges)
        {
            List<BidOrAskElement> selectedBidOrAskElements;

            if (transactionRequest.OrderType == OrderType.Buy)
            {
                var equippedAsksFromAllCryptoExchanges = EquipAsksWithExchangeNameInfo(cryptoExchanges);

                selectedBidOrAskElements =
                    _askCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(
                        transactionRequest.TransactionAmount, equippedAsksFromAllCryptoExchanges);
            }
            else
            {
                var equippedBidsFromAllCryptoExchanges = EquipBidsWithExchangeNameInfo(cryptoExchanges);

                selectedBidOrAskElements =
                    _bidCombinationSelector.PrepareListOfBidsOrAsksToSatisfyTransactionAmount(
                        transactionRequest.TransactionAmount, equippedBidsFromAllCryptoExchanges);
            }

            if (selectedBidOrAskElements.Any())
            {
                var hedgerTransactions = CreateHedgerTransactionsFromSelectedBidsOrAsks(selectedBidOrAskElements);

                return new RequestProcessorResult()
                { TransactionIsValid = true, HedgerTransactions = hedgerTransactions };
            }

            return new RequestProcessorResult()
            { TransactionIsValid = false, ErrorMessage = "Could not produce Hedger transactions because the selectedBid/Ask list was empty." };
        }


        /// <summary>
        /// Creates headger transactions list of Bid/Ask elements
        /// </summary>
        /// <param name="selectedBidsOrAsks">List of Asks/Bids</param>
        /// <returns>List of transactions for Hedger</returns>
        private List<HedgerTransaction> CreateHedgerTransactionsFromSelectedBidsOrAsks(List<BidOrAskElement> selectedBidsOrAsks)
        {
            var hedgerTransactions = new List<HedgerTransaction>();

            foreach (var singleBidOrAsk in selectedBidsOrAsks)
            {
                var hedgerTransaction = new HedgerTransaction
                {
                    CryptoExchange = singleBidOrAsk.CryptoExchangeName,
                    Order = singleBidOrAsk.Order
                };
                hedgerTransactions.Add(hedgerTransaction);
            }

            return hedgerTransactions;
        }


        /// <summary>
        /// Combines bids from all crypto exchanges into single list and
        /// equips each bid with cryptoExchange name
        /// </summary>
        /// <param name="cryptoExchanges">List of all crypto exchanges</param>
        /// <returns>List of bids from all cryptoExchanges with each bid being equipped with cryptoExchange name</returns>
        private List<Bid> EquipBidsWithExchangeNameInfo(List<CryptoExchange> cryptoExchanges)
        {
            var bidsFromAllCryptoExchanges = new List<Bid>();

            foreach (var cryptoExchange in cryptoExchanges)
            {
                foreach (var bid in cryptoExchange.OrderBook.Bids)
                {
                    var equippedBid = new Bid() { CryptoExchangeName = cryptoExchange.Name, Order = bid.Order };
                    bidsFromAllCryptoExchanges.Add(equippedBid);
                }
            }

            return bidsFromAllCryptoExchanges;
        }

        /// <summary>
        /// Combines asks from all crypto exchanges into single list and
        /// equips each ask with cryptoExchange name
        /// </summary>
        /// <param name="cryptoExchanges">List of all crypto exchanges</param>
        /// <returns>List of asks from all cryptoExchanges with each bid being equipped with cryptoExchange name</returns>
        private List<Ask> EquipAsksWithExchangeNameInfo(List<CryptoExchange> cryptoExchanges)
        {
            var asksFromAllCryptoExchanges = new List<Ask>();

            foreach (var cryptoExchange in cryptoExchanges)
            {
                foreach (var ask in cryptoExchange.OrderBook.Asks)
                {
                    var equippedAsk = new Ask() { CryptoExchangeName = cryptoExchange.Name, Order = ask.Order };
                    asksFromAllCryptoExchanges.Add(equippedAsk);
                }
            }

            return asksFromAllCryptoExchanges;
        }
    }
}