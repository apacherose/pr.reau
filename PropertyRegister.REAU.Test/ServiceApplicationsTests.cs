using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Persistence;
using PropertyRegister.REAU.Applications.Results;
using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Common.Persistence;
using PropertyRegister.REAU.Domain;
using PropertyRegister.REAU.Integration;
using PropertyRegister.REAU.Payments;
using PropertyRegister.REAU.Persistence;
using PropertyRegister.REAU.Test.Mocks;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace PropertyRegister.REAU.Test
{
    [TestClass]
    public class ServiceApplicationsTests
    {
        private ServiceProvider ServiceProvider;
        private ServiceCollection Services;

        public ServiceApplicationsTests()
        {
            var services = new ServiceCollection();
            services.AddTransient<ApplicationService>();
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<IServiceInstanceRepository, ServiceInstanceRepository>();
            services.AddTransient<IServiceOperationRepository, ServiceOperationRepository>();
            services.AddTransient<IApplicationDocumentRepository, ApplicationDocumentRepository>();

            services.AddTransient<IServiceActionRepository, ServiceInstanceActionEntityMock>();
            services.AddTransient<IDocumentService, DocumentServiceMock>();
            services.AddTransient<IApplicationServiceTypeCollection, ApplicationServiceTypesCollectionMock>();
            services.AddTransient<IPropertyRegisterClient, PropertyRegisterClientMock>();
            services.AddTransient<IPaymentIntegrationClient, PaymentIntegrationClientMock>();
            services.AddTransient<IIdempotentOperationExecutor, IdempotentOperationExecutor>();
            services.AddTransient<IPaymentManager, PaymentManager>();

            services.AddSingleton<IActionDispatcher, DummyActionDispatcherMock>();
            services.AddSingleton<IApplicationInfoResolver, ApplicationInfoResolver>();

            Services = services;
            ServiceProvider = services.BuildServiceProvider();
        }

        [TestInitialize]
        public void Init()
        {
            Thread.CurrentPrincipal = new TestPrincipal("1");
        }

        [TestCleanup]
        public void CleanUp()
        {
            ServiceProvider.GetRequiredService<IActionDispatcher>().Dispose();
        }

        [TestMethod]
        public async Task TestApplicationAcceptance()
        {
            ApplicationAcceptedResult response = null;

            string filename = "sample0102.xml";

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(filename);

            var applicationService = ServiceProvider.GetRequiredService<ApplicationService>();

            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                ms.Position = 0;

                response = await DbContextHelper.TransactionalOperationAsync(() =>
                {
                    return applicationService.AcceptApplicationAsync(Guid.NewGuid().ToString(), ms);
                });
            }

            Assert.IsTrue(response.ApplicationStatus == Applications.Models.ApplicationStatuses.Accepted);
        }

        [TestMethod]
        public async Task Test_MessageBus()
        {
            await ServiceProvider.GetRequiredService<IActionDispatcher>()
                .SendAsync("ApplicationAcceptance", (long)100);
        }

        [TestMethod]
        public void Test_ApplicationInfoResolver_AttachedDocuments()
        {
            var resolver = ServiceProvider.GetRequiredService<IApplicationInfoResolver>();
            
            using (var ms = File.OpenRead(@"C:\Users\vachev\Desktop\xsd-xml\xml\CertificateForPersonFromPropertyRegister-test.xml"))
            {
                var info = resolver.GetApplicationInfoFromXml(ms);

                Assert.IsTrue(info.AttachedDocumentIDs != null && info.AttachedDocumentIDs.Any());
            }                
        }
    }
}
