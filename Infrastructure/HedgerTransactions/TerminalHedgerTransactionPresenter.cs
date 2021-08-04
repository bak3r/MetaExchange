using System;
using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.HedgerTransactions
{
    public class TerminalHedgerTransactionPresenter : IHedgerTransactionPresenter
    {
        public void DisplayHedgerTransactions(List<HedgerTransaction> hedgerTransactions)
        {
            Console.WriteLine("#### HedgerTransactions #########################################");
            for (int i = 0; i < hedgerTransactions.Count; i++)
            {
                Console.WriteLine((i + 1) + ". ### CryptoExchangeId:" + hedgerTransactions[i].CryptoExchange + " Amount:" +
                                  hedgerTransactions[i].Order.Amount + " Price:" + hedgerTransactions[i].Order.Price
                                  + " Type:" + hedgerTransactions[i].Order.Type);

            }
        }
    }
}