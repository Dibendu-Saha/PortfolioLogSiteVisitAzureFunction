using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PortfolioLogSiteVisit
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            await host.RunAsync();
        }
    }
}
