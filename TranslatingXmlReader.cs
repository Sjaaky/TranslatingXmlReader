using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Tjip.Gateway.Services
{
    /// <summary>
    /// Translates xml while reading, so we don't need to do expensive parse, change, tostring roundtrips.
    /// </summary>
    public class TranslatingXmlReader : XmlTextReader
    {
        #region Constructors
        public TranslatingXmlReader(Stream input) : base(input)
        {
        }

        public TranslatingXmlReader(TextReader input) : base(input)
        {
        }

        public TranslatingXmlReader(string url) : base(url)
        {
        }

        public TranslatingXmlReader(Stream input, XmlNameTable nt) : base(input, nt)
        {
        }

        public TranslatingXmlReader(TextReader input, XmlNameTable nt) : base(input, nt)
        {
        }

        public TranslatingXmlReader(string url, Stream input) : base(url, input)
        {
        }

        public TranslatingXmlReader(string url, TextReader input) : base(url, input)
        {
        }

        public TranslatingXmlReader(string url, XmlNameTable nt) : base(url, nt)
        {
        }

        public TranslatingXmlReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context)
        {
        }

        public TranslatingXmlReader(string url, Stream input, XmlNameTable nt) : base(url, input, nt)
        {
        }

        public TranslatingXmlReader(string url, TextReader input, XmlNameTable nt) : base(url, input, nt)
        {
        }

        public TranslatingXmlReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context)
        {
        }

        protected TranslatingXmlReader()
        {
        }

        protected TranslatingXmlReader(XmlNameTable nt) : base(nt)
        {
        }
        #endregion

        public TranslatingXmlReader AddNamespaceUriTranslation(string from, string to)
        {
            namespaceUriTranslations.Add(from, to);
            return this;
        }

        public TranslatingXmlReader AddNameTranslation(string from, string to)
        {
            localNameTranslations.Add(from, to);
            return this;
        }

        public TranslatingXmlReader AddNameAndNsTranslation(string fromNs, string fromName, string toNs, string toName)
        {
            nameAndNsTranslations.Add((fromNs, fromName), (toNs, toName));
            return this;
        }

        public override string LookupNamespace(string prefix)
        {
            return base.LookupNamespace(prefix);
        }

        public override Type ValueType => base.ValueType;

        public override string Prefix
        {
            get
            {
                var prefix = ((IXmlNamespaceResolver)this).LookupPrefix(NamespaceURI);
                //this.GetNamespacesInScope(XmlNamespaceScope.All).TryGetValue()
                //if ( )
                return prefix ?? base.Prefix;
            }
        }

        //IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
        //{
        //    return this.GetNamespacesInScope(scope);
        //}

        //string IXmlNamespaceResolver.LookupNamespace(string prefix)
        //{
        //    return this.LookupNamespace(prefix);
        //}

        public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
        {
            return base.ReadElementContentAs(returnType, namespaceResolver);
        }

        public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
        {
            return base.ReadElementContentAs(returnType, namespaceResolver, localName, namespaceURI);
        }

        public override bool Read()
        {
            bool more = base.Read();
            // skip TP node
            if (NodeType == XmlNodeType.Element && LocalName == "TP")
            {
                while (more = base.Read())
                {
                    if (NodeType == XmlNodeType.EndElement && LocalName == "TP")
                    {
                        more = base.Read();
                        break;
                    }
                }
            }
            return more;
        }

        public override string NamespaceURI
        {
            get
            {
                if (nameAndNsTranslations.TryGetValue((base.NamespaceURI, base.LocalName), out var translatedName))
                {
                    return translatedName.Item1;
                }
                else
                {
                    if (namespaceUriTranslations.TryGetValue(base.NamespaceURI, out var translated))
                    {
                        return translated;
                    }
                    else
                    {
                        return base.NamespaceURI;
                    }
                }
            }
        }

        public override string LocalName
        {
            get
            {
                if (nameAndNsTranslations.TryGetValue((base.NamespaceURI, base.LocalName), out var translatedName))
                {
                    return translatedName.Item2;
                }
                else
                {
                    if (localNameTranslations.TryGetValue(base.LocalName, out var translated))
                    {
                        return translated;
                    }
                    else
                    {
                        return base.LocalName;
                    }
                }
            }
        }

        public override string Name
        {
            get
            {
                if (NodeType == XmlNodeType.Element)
                {
                    return base.Name;
                }
                else
                {
                    return base.Name;
                }
                // The Name needs translation as well, but it isn't used by the deserializer.
                throw new NotImplementedException();
            }
        }

        private readonly Dictionary<(string, string), (string, string)> nameAndNsTranslations = new Dictionary<(string, string), (string, string)>();
        private readonly Dictionary<string, string> localNameTranslations = new Dictionary<string, string>();
        private readonly Dictionary<string, string> namespaceUriTranslations = new Dictionary<string, string>();
    }

    public class TranslatingXmlNodeReader : XmlNodeReader
    {
        public TranslatingXmlNodeReader(XmlNode node) : base(node)
        {
        }


        public TranslatingXmlNodeReader AddNamespaceUriTranslation(string from, string to)
        {
            namespaceUriTranslations.Add(from, to);
            return this;
        }

        public TranslatingXmlNodeReader AddNameTranslation(string from, string to)
        {
            localNameTranslations.Add(from, to);
            return this;
        }

        public TranslatingXmlNodeReader AddNameAndNsTranslation(string fromNs, string fromName, string toNs, string toName)
        {
            nameAndNsTranslations.Add((fromNs, fromName), (toNs, toName));
            return this;
        }

        public override string NamespaceURI
        {
            get
            {
                if (nameAndNsTranslations.TryGetValue((base.NamespaceURI, base.LocalName), out var translatedName))
                {
                    return translatedName.Item1;
                }
                else
                {
                    if (namespaceUriTranslations.TryGetValue(base.NamespaceURI, out var translated))
                    {
                        return translated;
                    }
                    else
                    {
                        return base.NamespaceURI;
                    }
                }
            }
        }

        public override string LocalName
        {
            get
            {
                if (nameAndNsTranslations.TryGetValue((base.NamespaceURI, base.LocalName), out var translatedName))
                {
                    return translatedName.Item2;
                }
                else
                {
                if (localNameTranslations.TryGetValue(base.LocalName, out var translated))
                {
                    return translated;
                    }
                    else
                    {
                        return base.LocalName;
                    }
                }
            }
        }

        public override string Name
        {
            get
            {
                // The Name needs translation as well, but it isn't used by the deserializer.
                throw new NotImplementedException();
            }
        }

        private readonly Dictionary<(string, string), (string, string)> nameAndNsTranslations = new Dictionary<(string, string), (string, string)>();
        private readonly Dictionary<string, string> localNameTranslations = new Dictionary<string, string>();
        private readonly Dictionary<string, string> namespaceUriTranslations = new Dictionary<string, string>();
    }

    //public class TranslatingXmlNamespaceManager : XmlNamespaceManager
    //{
    //    public TranslatingXmlNamespaceManager(XmlNameTable nameTable) : base(nameTable)
    //    {
    //    }

    //    public override string LookupNamespace(string prefix)
    //    {
    //        return base.LookupNamespace(prefix);
    //    }

    //    public override string LookupPrefix(string uri)
    //    {
    //        return base.LookupPrefix(uri);
    //    }
    //}
}