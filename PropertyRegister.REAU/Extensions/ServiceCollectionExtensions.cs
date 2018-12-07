using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Persistence;
using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Common.Persistence;
using PropertyRegister.REAU.Integration;
using PropertyRegister.REAU.Nomenclatures;
using PropertyRegister.REAU.Payments;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationsPersistence(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IApplicationRepository, ApplicationRepository>()
                .AddTransient<IServiceInstanceRepository, ServiceInstanceRepository>()
                .AddTransient<IApplicationDocumentRepository, ApplicationDocumentRepository>()
                .AddTransient<IServiceActionRepository, ServiceActionRepository>();
        }

        public static IServiceCollection AddApplicationsAcceptance(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IApplicationAcceptanceService, ApplicationAcceptanceService>()
                .AddTransient<INomenclaturesProvider, NomenclaturesProvider>()
                .AddTransient<IApplicationInfoResolver, ApplicationInfoResolver>();
        }

        public static IServiceCollection AddApplicationsProcessing(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IApplicationProcessingService, ApplicationService>()
                .AddTransient<INomenclaturesProvider, NomenclaturesProvider>()
                .AddTransient<IPaymentManager, PaymentManager>();
        }

        public static IServiceCollection AddDocuments(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IDocumentDataRepository, DocumentDataRepository>()
                .AddTransient<IDocumentService, DocumentService>();
        }

        public static IServiceCollection AddREAUInfrastructureServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IActionDispatcher, DefaultActionDispatcher>()
                .AddTransient<IIdempotentOperationExecutor, IdempotentOperationExecutor>()
                .AddTransient<IServiceOperationRepository, ServiceOperationRepository>();
        }

        public static IServiceCollection AddIntegrationClients(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IPropertyRegisterClient, PropertyRegisterClient>()
                .AddTransient<IPaymentIntegrationClient, PaymentIntegrationClient>();
        }
    }
}
