using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface ICombinationSelector<T>
    {
        List<T> PrepareListOfBidsOrAsksToSatisfyTransactionAmount(decimal transactionRequestAmount,
            List<T> orderBookBidsOrAsks);
    }
}