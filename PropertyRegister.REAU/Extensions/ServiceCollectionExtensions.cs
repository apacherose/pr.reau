﻿using Microsoft.Extensions.DependencyInjection;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Persistence;
using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Common.Persistence;
using PropertyRegister.REAU.Nomenclatures;

namespace PropertyRegister.REAU.Extensions
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

        public static IServiceCollection AddDocuments(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IDocumentDataRepository, DocumentDataRepository>()
                .AddTransient<IDocumentService, DocumentService>();
        }

        public static IServiceCollection AddREAUInfrastructureServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IIdempotentOperationExecutor, IdempotentOperationExecutor>()
                .AddTransient<IServiceOperationRepository, ServiceOperationRepository>();
        }
    }
}
