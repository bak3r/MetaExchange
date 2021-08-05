using System.Collections.Generic;

namespace Core.Implementations.DTOs
{
    public class RequestProcessorResult
    {
        public bool TransactionIsValid { get; set; }
        public List<HedgerTransaction> HedgerTransactions { get; set; }

        public string ErrorMessage { get; set; }
    }
}