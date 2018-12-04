using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace PropertyRegister.REAU.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = CreateLogger();

            CreateWebHostBuilder(args)
                .Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();

        public static Logger CreateLogger() =>
            new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.With<UserIdentityEnricher>()
                .WriteTo.File(
                    path: @"C:\tmp\EPZEU\PR.REAU.Web.Api\log.txt", 
                    outputTemplate: "---------------------------------------------------------------------- {NewLine} {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{UserIdentity}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Hour)
                .CreateLogger();
    }

    public class UserIdentityEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
             => logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserIdentity", @"cnsys\vachev")); 
    }
}
