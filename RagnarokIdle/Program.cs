// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MGF_Lidgren;

var builder = new HostBuilder()
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         config.AddEnvironmentVariables();

         if (args != null)
         {
             config.AddCommandLine(args);
         }
     })
     .ConfigureServices((hostContext, services) =>
     {
         services.AddOptions();
         services.Configure<LidgrenConfig>(hostContext.Configuration.GetSection("Daemon"));

         services.AddSingleton<IHostedService, LidgrenServer>();
     })
     .ConfigureLogging((hostingContext, logging) => {
         logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
         logging.AddConsole();
     });

await builder.RunConsoleAsync();