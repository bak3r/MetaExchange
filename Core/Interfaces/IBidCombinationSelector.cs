using System.Collections.Generic;
using Core.Implementations.DTOs;

namespace Core.Interfaces
{
    public interface IBidCombinationSelector
    {
        public List<Bid> PrepareListOfBidsToSatisfyTransactionAmount(decimal transactionRequestAmount,
            List<Bid> orderBookBids);
    }
}