using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Tjip.Gateway.Proxies.Interpolis.AOV.Calculation;
using Tjip.Gateway.Services;
using Xunit;

namespace Tjip.Gateway.Tests.Domain
{
    public class Deserialize
    {
        private const string RootnodeName = "CalculatePremiumRequestType";
        private const string nsOperation = "http://www.tempuri.org/proposition/Operations/2019/10";
        private const string nsDatamodel = "http://www.tempuri.org/proposition/Datamodel/2019/10";

        [Fact]
        public void WithStringRootnodenameSucceeds()
        {
            var rootnodename = RootnodeName;
            var request = FileTo<CalculatePremiumRequestType>("Contractdocument.xml", rootnodename);
        }

        [Fact]
        public void WithTypeofRootnodenameSucceeds()
        {
            var rootnodename = typeof(CalculatePremiumRequestType).Name;
        
            var request = FileTo<CalculatePremiumRequestType>("Contractdocument.xml", rootnodename);
        }

        private static T FileTo<T>(string xmlFilename, string rootnodeName)
        {
            var xmlData = File.ReadAllText(xmlFilename);

            var tr = CreateXmlReader<T>(xmlData, rootnodeName);
            var request = XmlDeserialize<T>(tr);
            return request;
        }

        private static XmlReader CreateXmlReader<T>(string xmlData, string rootnodeName)
        {
            var nt = new NameTable();
            var ns = new XmlNamespaceManager(nt);
            var context = new XmlParserContext(nt, ns, string.Empty, XmlSpace.Default);

            rootnodeName = nt.Add(rootnodeName);
            var Contractdocument = nt.Add("Contractdocument");
            var AL = nt.Add("AL");
            var PP = nt.Add("PP");
            var XG = nt.Add("XG");
            var emptyNs = string.Empty;
            var tr = new TranslatingXmlReader(xmlData, XmlNodeType.Document, context)
                .AddNameAndNsTranslation(emptyNs, Contractdocument, emptyNs, rootnodeName)
                .AddNameAndNsTranslation(emptyNs, AL, nsOperation, AL)
                .AddNameAndNsTranslation(emptyNs, PP, nsOperation, PP)
                .AddNameAndNsTranslation(emptyNs, XG, nsOperation, XG)
                .AddNamespaceUriTranslation(emptyNs, nsDatamodel);

            return tr;
        }

        private static T XmlDeserialize<T>(XmlReader reader)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(reader);
        }
    }
}
