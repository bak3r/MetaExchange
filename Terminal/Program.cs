using System.Threading.Tasks;
using Core.Implementations;
using Core.Interfaces;
using Infrastructure.CryptoExchanges;
using Infrastructure.HedgerTransactions;
using Infrastructure.OrderBookRetrieval;
using Infrastructure.TransactionRequests;
using Microsoft.Extensions.DependencyInjection;

namespace Terminal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<BaseTransactionProcessor>().Run();
            serviceProvider.Dispose();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<IOrderBookRetriever, FileOrderBookRetriever>();
            services.AddTransient<ICryptoExchangeCreator, DummyCryptoExchangeCreator>();
            services.AddTransient<ICryptoExchangePresenter, TerminalCryptoExchangePresenter>();
            services.AddTransient<ITransactionRequestRetriever, DummyTransactionRequestRetriever>();
            services.AddTransient<ITransactionRequestPresenter, TerminalTransactionRequestPresenter>();
            services.AddTransient<ITransactionRequestProcessor, TransactionRequestProcessor>();
            services.AddTransient<IBuyTransactionRequestProcessor, SimpleBuyTransactionRequestProcessor>();
            services.AddTransient<ISellTransactionRequestProcessor, SimpleSellTransactionRequestProcessor>();
            services.AddTransient<IHedgerTransactionPresenter, TerminalHedgerTransactionPresenter>();

            services.AddTransient<BaseTransactionProcessor>();
        }
    }
}