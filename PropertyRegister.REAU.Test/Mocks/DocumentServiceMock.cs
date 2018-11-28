using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class DocumentServiceMock : IDocumentService
    {
        public Task DeleteDocumentAsync(string documentIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<DocumentData> GetDocumentAsync(string documentIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<List<DocumentData>> GetDocumentsAsync(List<string> documentIdentifiers)
        {
            throw new NotImplementedException();
        }

        public Task<DocumentData> SaveDocumentAsync(DocumentCreateRequest request)
        {
            return Task.FromResult(new DocumentData() { DocID = 100 });
        }
    }
}
