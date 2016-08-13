using System.Collections.Generic;
using System.Linq;
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
            var inputList = inputs.ToList();
            if (!inputList.Any())
            {
                return XDocument.Parse("<configuration></configuration>");
            }

            var result = inputList.First().Content;
            foreach (var source in inputList.Skip(1))
            {
                result = new XmlMerge(result, source, _options).Result;
            }
            return result;
        }
    }
}