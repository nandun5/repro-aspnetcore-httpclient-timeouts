
using HttpClientTimeouts.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace HttpClientTimeouts.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddHttpClient<IExternalService, HttpBinService>();
            services.AddSingleton<Processor>();
            services.AddLogging();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            Processor p = serviceProvider.GetService<Processor>();

            p.StartProcessing();

            System.Console.WriteLine("All Done.");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
