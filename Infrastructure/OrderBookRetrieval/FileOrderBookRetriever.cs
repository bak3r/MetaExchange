using System.Collections.Generic;
using System.IO;
using Core.Implementations.DTOs;
using Core.Interfaces;
using Newtonsoft.Json;

namespace Infrastructure.OrderBookRetrieval
{
    public class FileOrderBookRetriever : IOrderBookRetriever
    {
        // for simplicity hardcoding the file with path
        private string orderBooksFile = @"c:\temp\order_books_data.json";

        /// <summary>
        /// Retrieves multiple OrderBooks from file. Row timestamp is used as crypto exchange name for simplicity's sake.
        /// </summary>
        /// <param name="numberOfOrderBooksToRetrieve">Number of OrderBooks to retrieve from file.</param>
        /// <returns>Dictionary where key is CryptoExchange name and value is OrderBook read from file.</returns>
        public Dictionary<string, OrderBook> RetrieveOrderBooks(int numberOfOrderBooksToRetrieve)
        {
            var orderBooks = new Dictionary<string, OrderBook>();

            int lineCounter = 0;
            string lineContent;

            using (var reader = new StreamReader(orderBooksFile))
            {
                while (lineCounter < numberOfOrderBooksToRetrieve)
                {
                    lineCounter++;
                    lineContent = reader.ReadLine();

                    var indexOfFirstSquigly = lineContent.IndexOf('{');
                    var timestamp = lineContent.Substring(0, indexOfFirstSquigly).Trim();
                    var lineContentWithoutTimestamp = lineContent.Substring(indexOfFirstSquigly);
                    var deserializedOrderBook = JsonConvert.DeserializeObject<OrderBook>(lineContentWithoutTimestamp);
                    orderBooks.Add(timestamp, deserializedOrderBook);
                }
            }

            return orderBooks;
        }
    }
}