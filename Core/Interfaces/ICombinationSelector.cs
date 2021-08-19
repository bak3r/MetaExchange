using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface ICombinationSelector<T>
    {
        List<BidOrAskElement> PrepareListOfBidsOrAsksToSatisfyTransactionAmount(decimal transactionRequestAmount,
            List<T> orderBookBidsOrAsks);

    }
}