using PropertyRegister.REAU.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Common
{
    public class DocumentRequest
    {
        // + some file metadata ...
        public string ContentType { get; set; }

        public Stream Content { get; set; }
    }

    public interface IDocumentService
    {
        Task<DocumentData> SaveDocumentAsync(DocumentRequest request);
        Task DeleteDocumentAsync(string documentIdentifier);
        Task<DocumentData> GetDocumentAsync(string documentIdentifier);
        Task<List<DocumentData>> GetDocumentsAsync(List<string> documentIdentifiers);
    }

    public class DocumentService : IDocumentService
    {

        public Task<DocumentData> SaveDocumentAsync(DocumentRequest request)
        {
            return null;
        }

        public Task DeleteDocumentAsync(string documentIdentifier)
        {
            return null;
        }

        public Task<DocumentData> GetDocumentAsync(string documentIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<List<DocumentData>> GetDocumentsAsync(List<string> documentIdentifiers)
        {
            throw new NotImplementedException();
        }
    }
}
