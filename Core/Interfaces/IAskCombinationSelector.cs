using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IAskCombinationSelector
    {
        List<Ask> PrepareListOfAsksToSatisfyTransactionAmount(decimal transactionRequestAmount, List<Ask> orderBookAsks);
    }
}