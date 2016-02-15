using System.Collections.Generic;
using System.Xml.Linq;

namespace ConfigMerge.Core.Merging
{
    public class XmlMerger
    {
        private readonly TransformOptions _options;

        public XmlMerger(TransformOptions options)
        {
            _options = options;
        }

        public XDocument Merge(IEnumerable<XmlSource> inputs)
        {
            var result = XDocument.Parse("<configuration></configuration>");
            foreach (var xmlSource in inputs)
            {
                result = new XmlMerge(result, xmlSource, _options).Result;
            }
            return result;
        }
    }
}