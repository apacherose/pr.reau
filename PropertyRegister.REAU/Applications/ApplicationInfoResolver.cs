using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ApplicationXmlInfo GetApplicationInfoFromXml(Stream xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xml);

            string docNamespace = doc.DocumentElement.NamespaceURI;

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("reau", "reau.pr.bg");

            var mainAppNumberNodes = doc.DocumentElement.SelectNodes("//reau:MainApplicationNumber", nsmgr);
            string mainAppNumber = mainAppNumberNodes.Count == 1 ? mainAppNumberNodes[0].InnerText : null;

            return new ApplicationXmlInfo()
            {
                XmlNamespace = docNamespace,
                MainApplicationNumber = mainAppNumber
            };
        }        
    }
}
