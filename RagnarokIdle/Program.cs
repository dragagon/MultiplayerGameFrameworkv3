// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MGF_Lidgren;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using MultiplayerGameFramework;
using Autofac.Configuration;

var builder = new HostBuilder()
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         config.AddEnvironmentVariables();

         if (args != null)
         {
             config.AddCommandLine(args);
         }

         config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{args[0]}.json")
                .AddJsonFile($"{args[1]}.json")
                .Build();
     })
     .UseServiceProviderFactory(new AutofacServiceProviderFactory())
     .ConfigureContainer<ContainerBuilder>((hostContext, builder) =>
     {
         builder.RegisterModule(new ConfigurationModule(hostContext.Configuration));
     })
     .ConfigureLogging((hostingContext, logging) => {
         logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
         logging.AddConsole();
     });

await builder.RunConsoleAsync();