using System;
using System.Collections.Generic;
using Core.Implementations.DTOs;
using Core.Implementations.Enums;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.TransactionRequests
{
    public class JsonTransactionRequestRetriever : ITransactionRequestRetriever
    {
        private readonly IConfiguration _configuration;

        public JsonTransactionRequestRetriever(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves single transaction request from configuration file.
        /// </summary>
        /// <returns>List containing a single transaction request.</returns>
        public List<TransactionRequest> RetrieveTransactionsForProcessing()
        {
            var transactionRequest = new TransactionRequest();
            var transactionRequestAmountParsedSuccessfully = decimal.TryParse(_configuration["TransactionRequest:Amount"], out var parsedTransactionRequestAmount);
            if (transactionRequestAmountParsedSuccessfully)
                transactionRequest.TransactionAmount = parsedTransactionRequestAmount;

            var orderTypeParsedSuccessfully = Enum.TryParse(_configuration["TransactionRequest:OrderType"],
                out OrderType parsedOrderType);
            if (orderTypeParsedSuccessfully)
                transactionRequest.OrderType = parsedOrderType;

            var transactionRequests = new List<TransactionRequest> {transactionRequest};

            return transactionRequests;
        }
    }
}