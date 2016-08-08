using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ConfigMerge.Core.Merging
{
    public class UniqueKey
    {
        public string ElementName { get; }
        public string[] Path { get; }
        public string AttributeName { get; }

        public UniqueKey(string attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute))
            {
                throw new ArgumentNullException(nameof(attribute));
            }
            var parts = attribute.Split('/');
            AttributeName = parts.Last();

            if (parts.Length > 1)
            {
                Path = parts.Take(parts.Length - 1).ToArray();
                ElementName = parts.First();
            }
            else
            {
                Path = new string[0];
            }
        }

        public static implicit operator UniqueKey(string attribute)
        {
            return new UniqueKey(attribute);
        }

        public KeyValue GetUniqueKey(XElement element)
        {
            var childElement = GetChildElement(element);
            var attrib = childElement.Attribute(AttributeName);
            if (attrib == null)
            {
                throw new ApplicationException($"Did not find attribute {AttributeName} in {element}");
            }
            return new KeyValue(AttributeName, attrib.Value);
        }

        private XElement GetChildElement(XElement element)
        {
            if (Path.Length > 0)
            {
                var path = string.Join("/", Path.Skip(1));
                var childElement = element.XPathSelectElement(path);
                
                
                if (childElement == null)
                {
                    throw new ApplicationException($"Did not find element {path} in {element}");
                }
                return childElement;
            }
            return element;
        }
    }
}