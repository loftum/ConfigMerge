using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ConfigMerge.Common;

namespace ConfigMerge.Core.Merging
{
    public class ElementMeta
    {
        public XElement Element { get; }
        public KeyValue UniqueKey { get; }
        public bool ShouldDelete { get; }

        private readonly Func<XElement, KeyValue> _getUniqueKey;
        private readonly TransformOptions _options;

        public ElementMeta(XElement element, TransformOptions options)
        {
            _options = options;
            Element = element;
            _getUniqueKey = DetermineUniqueKey(element);
            if (_getUniqueKey != null)
            {
                UniqueKey = _getUniqueKey(element);
            }
            ShouldDelete = DetermineShouldDelete(element);
        }

        public XElement GetMatch(IEnumerable<XElement> elements)
        {
            if (elements == null)
            {
                return null;
            }
            var matches = GetCandidates(elements);
            if (UniqueKey == null && matches.Length > 1)
            {
                throw new ApplicationException($"Cannot merge into file containing multiple undelimited equal siblings, needs attribute with name: {_options.UniqueAttributes.FriendlyCommaSeparated("or")}. The offender is: {Element}");
            }
            if (matches.Length > 1)
            {
                throw new ApplicationException($"Cannot merge into file containing multiple equal siblings with same identifier, needs unique value for attribute with name: {_options.UniqueAttributes.FriendlyCommaSeparated("or")}. The offender is: {Element}");
            }
            return matches.SingleOrDefault();
        }

        private XElement[] GetCandidates(IEnumerable<XElement> elements)
        {
            var candidates = elements.Where(e => e.Name == Element.Name);
            if (UniqueKey != null)
            {
                candidates = candidates.Where(c => UniqueKey.Equals(_getUniqueKey(c)));
            }
            return candidates.ToArray();
        }

        private bool DetermineShouldDelete(XElement element)
        {
            var deleteattrib = element.Attributes(_options.DeleteKeyword).ToArray();
            return deleteattrib.Length == 1 && bool.Parse(deleteattrib.First().Value);
        }

        private Func<XElement, KeyValue> DetermineUniqueKey(XElement element)
        {
            var uniqueKeys = _options.UniqueAttributes.Select(a => new UniqueKey(a)).ToArray();
            var specific = uniqueKeys.FirstOrDefault(k => string.Equals(element.Name.LocalName, k.ElementName, StringComparison.InvariantCultureIgnoreCase)) ??
                uniqueKeys.FirstOrDefault(u => element.Attributes(u.AttributeName).Any());

            if (specific != null)
            {
                return specific.GetUniqueKey;
            }
            return null;
        }
    }
}