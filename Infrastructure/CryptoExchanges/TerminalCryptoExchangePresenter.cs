using System;
using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Interfaces;

namespace Infrastructure.CryptoExchanges
{
    public class TerminalCryptoExchangePresenter : ICryptoExchangePresenter
    {
        public void OutputCryptoExchangesInfo(List<CryptoExchange> cryptoExchanges)
        {
            Console.WriteLine("#### CryptoExchanges ############################################");
            foreach (var cryptoExchange in cryptoExchanges)
            {
                System.Console.WriteLine("CryptoExchangeName: " + cryptoExchange.Name + " BalanceEur:" +
                                         cryptoExchange.BalanceEur + " BalanceBtc:" + cryptoExchange.BalanceBtc +
                                         " NrOfBids:" + cryptoExchange.OrderBook.Bids.Count +
                                         " NrOfAsks:" + cryptoExchange.OrderBook.Asks.Count);
            }
        }
    }
}