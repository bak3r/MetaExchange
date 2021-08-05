using System;
using System.Collections.Generic;
using System.Linq;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.HedgerTransactions
{
    public class TerminalHedgerTransactionPresenter : IHedgerTransactionPresenter
    {
        public void DisplayHedgerTransactions(List<HedgerTransaction> hedgerTransactions)
        {
            Console.WriteLine("#### HedgerTransactions #########################################");
            if (hedgerTransactions != null)
            {
                if (hedgerTransactions.Any())
                {
                    for (int i = 0; i < hedgerTransactions.Count; i++)
                    {
                        // Hedger transactions are basically taken/copied from Orderbook's list of Asks/Bids
                        // This inversion is made to instruct Hedger with proper inverse actions.
                        // Example: If hedger transactions are taken from Asks (which is what sellers are selling),
                        // the action for Hedger must be inverse of that => buy.
                        // Since this is only for printing actions the inversion is done here. Otherwise i would
                        // handle it in the hedger transactions creation process. 
                        var orderTypeInOrderbook = hedgerTransactions[i].Order.Type;
                        string orderForHedgerMustBeInverse = string.Empty;
                        switch (orderTypeInOrderbook)
                        {
                            case "Sell":
                                orderForHedgerMustBeInverse = "Buy";
                                break;
                            case "Buy":
                                orderForHedgerMustBeInverse = "Sell";
                                break;
                        }

                        Console.WriteLine((i + 1) + ". ### CryptoExchangeId:" + hedgerTransactions[i].CryptoExchange + " Amount:" +
                                          hedgerTransactions[i].Order.Amount + " Price:" + hedgerTransactions[i].Order.Price
                                          + " Type:" + orderForHedgerMustBeInverse);

                    }
                }
            }
            
        }
    }
}