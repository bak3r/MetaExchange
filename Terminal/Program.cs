using System.Threading.Tasks;
using Core.Implementations;
using Core.Implementations.DTOs;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.CryptoExchanges;
using Infrastructure.HedgerTransactions;
using Infrastructure.OrderBookRetrieval;
using Infrastructure.TransactionRequests;
using Microsoft.Extensions.Configuration;
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
            serviceProvider.GetService<MetaExchangeCoordinator>().Run();
            serviceProvider.Dispose();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddTransient<IOrderBookRetriever, FileOrderBookRetriever>();
            services.AddTransient<ICryptoExchangeCreator, DummyCryptoExchangeCreator>();
            services.AddTransient<ICryptoExchangePresenter, TerminalCryptoExchangePresenter>();
            services.AddTransient<ITransactionRequestRetriever, JsonTransactionRequestRetriever>();
            services.AddTransient<ITransactionRequestPresenter, TerminalTransactionRequestPresenter>();
            services.AddTransient<ITransactionRequestProcessor, TransactionRequestProcessor>();
            services.AddTransient<IHedgerTransactionPresenter, TerminalHedgerTransactionPresenter>();
            services.AddTransient<ICombinationSelector<Ask>, CombinationSelector<Ask>>();
            services.AddTransient<ICombinationSelector<Bid>, CombinationSelector<Bid>>();
            services.AddSingleton<IExchangeBalanceTracker, SimpleExchangeBalanceTracker>();
            services.AddTransient<MetaExchangeCoordinator>();
        }
    }
}