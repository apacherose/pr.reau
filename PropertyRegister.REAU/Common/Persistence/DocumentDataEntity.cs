using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using PropertyRegister.REAU.Common.Models;
using PropertyRegister.REAU.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace PropertyRegister.REAU.Common.Persistence
{
    public class DocumentDataSearchCriteria
    {
        public List<long> DocumentDataIDs { get; set; }
        public List<string> Identifiers { get; set; }
    }

    public interface IDocumentDataEntity : IEntity<DocumentData, Guid, DocumentDataSearchCriteria>
    {
        Stream ReadContent(string documentIdentifier);
    }

    public class DocumentDataEntity : EntityBase<DocumentData, Guid, DocumentDataSearchCriteria, CommonDataContext>, IDocumentDataEntity
    {
        public DocumentDataEntity()
            : base(false)
        {
        }

        protected override void CreateInternal(CommonDataContext context, DocumentData item)
        {
            item.Identifier = item.Identifier ?? Guid.NewGuid().ToString();
            
            using (var blobContent = new OracleBlob(context.DbConnection as OracleConnection))
            {
                item.Content.CopyTo(blobContent);

                context.DocumentDataCreate(item.Identifier, item.ContentType, item.Filename, item.IsTemporal, blobContent, out long documentId);
                item.DocID = documentId;
            }
        }

        protected override void UpdateInternal(CommonDataContext context, DocumentData item) =>
            context.DocumentDataUpdate(item.Identifier, item.ContentType, item.Filename, item.IsTemporal);

        protected override void DeleteInternal(CommonDataContext context, Guid key) =>
            context.DocumentDataDelete(key);
        
        protected override IEnumerable<DocumentData> SearchInternal(CommonDataContext context, DocumentDataSearchCriteria searchCriteria)
        {
            IEnumerable<DocumentData> res;

            using (var data = context.DocumentDataSearch(
                ToStringCollection(searchCriteria.DocumentDataIDs),
                ToStringCollection(searchCriteria.Identifiers), 1, 20, out int? count))
            {
                res = data.ReadToList<DocumentData>();
            }

            return res;
        }

        public Stream ReadContent(string documentIdentifier)
        {
            return new DeferredDbStream(() => {

                var dbContext = CreateDbContext();
                var dataCtx = CreateDataContext(dbContext);

                return new Tuple<DbContext, Stream>(dbContext, new WrappedBinaryStream(dataCtx.DocumentContentGet(documentIdentifier)));
            });
        }

        public static string ToStringCollection(IEnumerable items, char separator = ',')
        {
            if (items == null)
                return null;

            StringBuilder res = new StringBuilder();

            foreach (var item in items)
            {
                res.AppendFormat("{0}{1}", item, separator);
            }

            return
                res.Length > 0 ?
                res.ToString().Trim(separator) : null;
        }
    }   
}
