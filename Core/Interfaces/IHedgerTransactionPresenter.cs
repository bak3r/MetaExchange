using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IHedgerTransactionPresenter
    {
        void DisplayHedgerTransactions(List<HedgerTransaction> hedgerTransactions);
    }
}