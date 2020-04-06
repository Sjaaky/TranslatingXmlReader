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
            if (rootnodename == RootnodeName)
            {
                //rootnodename = RootnodeName; // uncomment to let this succeed
            }
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

            var emptyNs = string.Empty;
            var tr = new TranslatingXmlReader(xmlData, XmlNodeType.Document, context)
                .AddNameAndNsTranslation(emptyNs, "Contractdocument", emptyNs, rootnodeName)
                .AddNameAndNsTranslation(emptyNs, "AL", "http://www.tempuri.org/proposition/Operations/2019/10", "AL")
                .AddNameAndNsTranslation(emptyNs, "PP", "http://www.tempuri.org/proposition/Operations/2019/10", "PP")
                .AddNameAndNsTranslation(emptyNs, "XG", "http://www.tempuri.org/proposition/Operations/2019/10", "XG")
                .AddNamespaceUriTranslation(emptyNs, "http://www.tempuri.org/proposition/Datamodel/2019/10");

            return tr;
        }

        private static T XmlDeserialize<T>(XmlReader reader)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(reader);
        }
    }
}
