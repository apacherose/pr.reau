using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Nomenclatures;
using PropertyRegister.REAU.Web.Api.Test;
using Rebus.ServiceProvider;
using Serilog;

namespace PropertyRegister.REAU.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDocuments()
                .AddApplicationsAcceptance()
                .AddApplicationsPersistence()
                .AddREAUInfrastructureServices();

            // dummies ...
            services.AddTransient<INomenclaturesProvider, NomenclaturesProviderDummy>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });

            services.AddRebusWithOracleTransport(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UsePrincipalMiddleware();

            app.UseHttpsRedirection();
            app.UseRebus();
            app.UseMvc();
        }
    }
}
