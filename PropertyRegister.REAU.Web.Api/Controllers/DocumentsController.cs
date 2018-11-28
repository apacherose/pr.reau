using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyRegister.REAU.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Web.Api.Controllers
{
    public class DocumentsController : BaseApiController
    {
        private readonly IDocumentService DocumentService;

        public DocumentsController(IDocumentService documentService)
        {
            DocumentService = documentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument(IFormFile file)
        {
            var documentResult = await DocumentService.SaveDocumentAsync(new DocumentCreateRequest()
            {
                ContentType = file.ContentType,
                Filename = file.FileName,
                Content = file.OpenReadStream()
            });

            return Ok(new {
                docIdentifier = documentResult.DocumentIdentifier
            });
        }

        [Route("{documentIdentifier}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteDocument(string documentIdentifier)
        {
            await DocumentService.DeleteDocumentAsync(documentIdentifier);

            return Ok();
        }

        [Route("{documentIdentifier}")]
        [HttpGet]
        public async Task<IActionResult> GetDocument(string documentIdentifier)
        {
            var docs = await DocumentService.GetDocumentsAsync(new List<string>() { documentIdentifier }, true);
            var doc = docs.SingleOrDefault();

            if (doc != null)
            {
                return File(fileStream: doc.Content, contentType: doc.ContentType, fileDownloadName: doc.Filename);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
