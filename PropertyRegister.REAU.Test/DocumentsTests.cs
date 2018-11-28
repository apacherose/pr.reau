using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyRegister.REAU.Common.Models;
using PropertyRegister.REAU.Common.Persistence;
using PropertyRegister.REAU.Persistence;
using PropertyRegister.REAU.Test.Mocks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test
{
    [TestClass]
    public class DocumentsTests
    {
        private ServiceProvider ServiceProvider;
        private ServiceCollection Services;

        public DocumentsTests()
        {
            var services = new ServiceCollection();
            services.AddTransient<IDocumentDataEntity, DocumentDataEntity>();

            Services = services;
            ServiceProvider = services.BuildServiceProvider();
        }

        [TestInitialize]
        public void Init()
        {
            Thread.CurrentPrincipal = new TestPrincipal("1");
        }

        [TestMethod]
        public async Task Test_DocumentData()
        {
            await DbContextHelper.TransactionalOperationAsync(() =>
            {
                string filename = @"C:\Users\vachev\Desktop\scripts.sql";
                var entity = ServiceProvider.GetRequiredService<IDocumentDataEntity>();

                using (var fs = File.OpenRead(filename))
                {
                    var document = new DocumentData()
                    {
                        ContentType = "text/xml",
                        Filename = "test.txt",
                        IsTemporal = true,
                        Content = fs
                    };

                    entity.Create(document);

                    Assert.IsTrue(document.DocID.HasValue && document.Identifier.HasValue);

                    document.IsTemporal = false;

                    entity.Update(document);

                    var documentFound = entity.Search(new DocumentDataSearchCriteria() { Identifiers = new List<System.Guid>() { document.Identifier.Value } }).SingleOrDefault();

                    Assert.IsTrue(documentFound != null && documentFound.IsTemporal == false);
                }
                return Task.CompletedTask;
            });            
        }
    }
}
