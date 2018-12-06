using PropertyRegister.REAU.Common.Models;
using PropertyRegister.REAU.Common.Persistence;
using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Common
{
    public class DocumentCreateRequest
    {
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }

    public class DocumentCreateResult
    {
        public long DocumentDataID { get; set; }
        public string DocumentIdentifier { get; set; }
    }

    public interface IDocumentService
    {
        Task<DocumentCreateResult> SaveDocumentAsync(DocumentCreateRequest request);
        Task DeleteDocumentAsync(string documentIdentifier);
        Task<IEnumerable<DocumentData>> GetDocumentsAsync(List<string> documentIdentifiers, bool loadContent = false);
    }

    public class DocumentService : IDocumentService
    {
        private readonly IDocumentDataRepository DocumentDataRepository;

        public DocumentService(IDocumentDataRepository documentDataRepository)
        {
            DocumentDataRepository = documentDataRepository;
        }

        public Task<DocumentCreateResult> SaveDocumentAsync(DocumentCreateRequest request)
        {
            return DbContextHelper.TransactionalOperationAsync(() => SaveDocumentAsyncInternal(request));
        }

        public Task DeleteDocumentAsync(string documentIdentifier)
        {
            if (!Guid.TryParse(documentIdentifier, out Guid guid))
                throw new ArgumentException("documentIdentifier not valid GUID!");

            DocumentDataRepository.Delete(guid);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<DocumentData>> GetDocumentsAsync(List<string> documentIdentifiers, bool loadContent = false)
        {
            var docs = DocumentDataRepository.Search(new DocumentDataSearchCriteria()
            {
                Identifiers = documentIdentifiers
            });

            if (loadContent)
            {
                foreach (var doc in docs)
                {
                    doc.Content = DocumentDataRepository.ReadContent(doc.Identifier);
                }
            }

            return Task.FromResult(docs);
        }

        private Task<DocumentCreateResult> SaveDocumentAsyncInternal(DocumentCreateRequest request)
        {
            var documentData = new DocumentData()
            {
                Identifier = Guid.NewGuid().ToString(),
                ContentType = request.ContentType,
                Filename = request.Filename,
                IsTemporal = true,
                Content = request.Content
            };

            DocumentDataRepository.Create(documentData);

            return Task.FromResult(new DocumentCreateResult()
            {
                DocumentDataID = documentData.DocID.Value,
                DocumentIdentifier = documentData.Identifier.ToString()
            });
        }
    }
}
