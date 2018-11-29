using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PropertyRegister.REAU.Applications
{
    public interface IApplicationInfoResolver
    {
        ApplicationXmlInfo GetApplicationInfoFromXml(Stream xml);
    }

    public class ApplicationXmlInfo
    {
        public string XmlNamespace { get; set; }
        public string MainApplicationNumber { get; set; }
        public decimal? PaymentObligationAmount { get; set; }
        public IEnumerable<string> AttachedDocumentIDs { get; set; }
    }

    public class ApplicationInfoResolver : IApplicationInfoResolver
    {
        private const string _docNamespacePrefix = "__docns";

        public ApplicationXmlInfo GetApplicationInfoFromXml(Stream xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xml);

            XmlNamespaceManager nsmgr = BuildDocumentNamespaceManager(doc.NameTable, doc.DocumentElement.NamespaceURI);

            string mainAppNumber = doc.SelectSingleNode($"//{_docNamespacePrefix}:MainApplicationNumber", nsmgr)?.InnerText;

            var attDocsNodes = doc.SelectNodes("//att0:AttachedDocument/att1:DocumentUniqueId", nsmgr);

            List<string> docIdentifiers = new List<string>();
            foreach (XmlNode attDoc in attDocsNodes)
            {
                docIdentifiers.Add(attDoc.InnerText);
            }

            return new ApplicationXmlInfo()
            {
                XmlNamespace = doc.DocumentElement.NamespaceURI,
                MainApplicationNumber = mainAppNumber,
                AttachedDocumentIDs = docIdentifiers
            };
        }

        private XmlNamespaceManager BuildDocumentNamespaceManager(XmlNameTable xmlNameTable, string documentNamespace)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlNameTable);
            nsmgr.AddNamespace(_docNamespacePrefix, documentNamespace);
            nsmgr.AddNamespace("att0", "AttachedDocuments");
            nsmgr.AddNamespace("att1", "AttachedDocument");

            return nsmgr;
        }
    }
}
