using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Persistence;
using PropertyRegister.REAU.Applications.Results;
using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Common.Persistence;
using PropertyRegister.REAU.Domain;
using PropertyRegister.REAU.Integration;
using PropertyRegister.REAU.Test.Mocks;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PropertyRegister.REAU.Test
{
    [TestClass]
    public class ApplicationTests
    {
        private ServiceProvider ServiceProvider;
        private ServiceCollection Services;

        public ApplicationTests()
        {
            var services = new ServiceCollection();
            services.AddTransient<ApplicationService>();
            services.AddTransient<IApplicationRepository, ApplicationEntityMock>();
            services.AddTransient<IDocumentService, DocumentServiceMock>();
            services.AddTransient<IPropertyRegisterClient, PropertyRegisterClientMock>();
            services.AddTransient<IPaymentIntegrationClient, PaymentIntegrationClientMock>();
            services.AddSingleton<DbDataSets>();

            Services = services;
            ServiceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void TestMethod1()
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load("sample.xml");

            var obj = (Application)Deserialize(doc, typeof(Application));
        }

        [TestMethod]
        public async Task Test_Application_Save()
        {
            var response = await DoApplicationSave("sample0101.xml");

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Test_FreeApplication_Save()
        {
            var response = await DoApplicationSave("sample0102.xml");

            Assert.IsTrue(response != null
                && response.PaymentAmount == 0m
                && response.ApplicationStatus == Applications.Models.ApplicationStatuses.Completed
                && !string.IsNullOrEmpty(response.RegisterOutcomeNumber));
        }

        [TestMethod]
        public async Task Test_Correction_Save()
        {
            var response = await DoApplicationSave("correction01.xml");

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Test_FreeReport_Save()
        {
            var response = await DoApplicationSave("report01.xml");

            Assert.IsTrue(response.ApplicationStatus == Applications.Models.ApplicationStatuses.Completed
                && (response.PaymentAmount == 0m || response.PaymentAmount == null));
        }

        [TestMethod]
        public async Task Test_PaidReport_Save()
        {
            var response = await DoApplicationSave("report02.xml");

            Assert.IsTrue(response.ApplicationStatus == Applications.Models.ApplicationStatuses.WaitingPayment
                && response.PaymentAmount.GetValueOrDefault() > 0m);
        }

        public Task<ApplicationProcessedResult> DoApplicationSave(string filename, string operationID = null, ServiceProvider serviceProvider = null)
        {
            return Task.FromResult(new ApplicationProcessedResult());

            //XmlDocument doc = new XmlDocument();
            //doc.PreserveWhitespace = true;
            //doc.Load(filename);

            //var portalOperationID = operationID ?? Guid.NewGuid().ToString();
            //var applicationService = (serviceProvider ?? ServiceProvider).GetRequiredService<ApplicationService>();

            //ApplicationResponse response = null;

            //using (var ms = new MemoryStream())
            //{
            //    doc.Save(ms);
            //    ms.Position = 0;

            //    response = await applicationService.SaveAsync(portalOperationID, ms);
            //}
            //return response;
        }

        [TestMethod]
        public void Test_DeserializeConcreteType()
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load("sample0101.xml");

            var obj = (CertificateForPerson)Deserialize(doc, typeof(CertificateForPerson));
        }

        [TestMethod]
        public void Test_Application_CRUD_Create()
        {
            new ApplicationRepository().Create(new Applications.Models.Application());
        }

        [TestMethod]
        public void Test_ServiceOperation_Search()
        {
            var entity = new ServiceOperationRepository();

            var data = entity.Search(new ServiceOperationSearchCriteria()
            {
                ServiceOperationType = Common.Models.ServiceOperationTypes.AcceptServiceApplication
            }).ToList();

            Assert.IsTrue(data != null);
        }

        [TestMethod]
        public void Test_ServiceOperation_Create()
        {
            var entity = new ServiceOperationRepository();
            
            var operation = new Common.Models.ServiceOperation()
            {
                OperationID = Guid.NewGuid().ToString(),
                ServiceOperationType = Common.Models.ServiceOperationTypes.AcceptServiceApplication
            };
            entity.Create(operation);
            
            Assert.IsTrue(operation.ServiceOperationID.HasValue);
        }



        private object Deserialize(XmlDocument doc, Type type)
        {
            var serializer = new XmlSerializer(type);

            using (StringReader rdr = new StringReader(doc.OuterXml))
            {
                return serializer.Deserialize(rdr);
            }
        }
    }

    [XmlType(AnonymousType = true, Namespace = "reau.pr.bg")]
    [XmlRoot("Application", Namespace = "reau.pr.bg", IsNullable = false)]
    public class Application
    {
        public Header Header { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("AttachedDocument")]
        public AttachedDocument[] AttachedDocuments { get; set; }
    }

    public class Header
    {
        public ServiceInfo ServiceInfo { get; set; }
        public string OfficeID { get; set; }
        public Applicant Applicant { get; set; }
    }

    public class ServiceInfo
    {
        public string Code { get; set; }
        public decimal Obligation { get; set; }
    }

    public class Applicant
    {
        public string CIN { get; set; }
    }

    public class AttachedDocument
    {
        public Guid BackOfficeGUID { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "reau.pr.bg/CertificateForPerson")]
    [XmlRoot("CertificateForPerson", Namespace = "reau.pr.bg/CertificateForPerson", IsNullable = false)]
    public class CertificateForPerson : ApplicationBase
    {
        public string Type { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "reau.pr.bg")]
    public class ApplicationBase
    {
        public Header Header { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("AttachedDocument")]
        public AttachedDocument[] AttachedDocuments { get; set; }
    }
}
