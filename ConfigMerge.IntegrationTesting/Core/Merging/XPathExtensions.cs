using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ConfigMerge.IntegrationTesting.Core.Merging
{
    public static class XPathExtensions
    {
        public static XElement XPathSelectElementOrThrow(this XNode node, string expression)
        {
            var element = node.XPathSelectElement(expression);
            if (element == null)
            {
                throw new InvalidOperationException($"Could not find {expression} in {node}");
            }
            return element;
        }
    }
}