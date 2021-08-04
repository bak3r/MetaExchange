using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface ICryptoExchangePresenter
    {
        void OutputCryptoExchangesInfo(List<CryptoExchange> cryptoExchanges);
    }
}