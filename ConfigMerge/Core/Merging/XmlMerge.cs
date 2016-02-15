using System;
using System.Linq;
using System.Xml.Linq;
using ConfigMerge.Common;
using ConfigMerge.Logging;

namespace ConfigMerge.Core.Merging
{
    public class XmlMerge
    {
        private static readonly ILogger Log = Logger.For<XmlMerge>();

        public XDocument Result { get; }
        private readonly XmlSource _source;
        private readonly TransformOptions _options;

        public XmlMerge(XDocument result, XmlSource source, TransformOptions options)
        {
            Result = result;
            _source = source;
            _options = options;
            Merge(source.Content.Root);
        }

        private void Merge(XElement input)
        {
            Merge(Result.Root, input);
        }

        private void Merge(XElement template, XElement input)
        {
            if (template == null)
            {
                return;
            }
            if (ShouldDeleteNode(input))
            {
                Log.Trace($"{_source}: Remove {input}");
                template.Remove();
                return;
            }
            MergeAttributes(template, input);

            if (!input.HasElements)
            {
                if (input.Value == _options.DeleteKeyword)
                {
                    template.Value = string.Empty;
                }
                else if (!string.IsNullOrEmpty(input.Value))
                {
                    template.Value = input.Value;
                }
                return;
            }

            var templateElements = template.Elements().ToArray();
            var dataElements = input.Elements();
            foreach (var dataNode in dataElements)
            {
                var uniqueKey = GetUniqueKey(dataNode);
                var shouldDeleteDataNode = ShouldDeleteNode(dataNode);
                var matchingNodes = templateElements.Where(x => x.Name == dataNode.Name).ToArray();
                if (!matchingNodes.Any() && !shouldDeleteDataNode)
                {
                    Log.Trace($"{_source.FilePath}: Adding {dataNode}");
                    if (_options.EnableTrace)
                    {
                        dataNode.SetAttributeValue("TRACE", TraceText("CREATED"));
                    }
                    template.Add(dataNode);
                    continue;
                }

                if (uniqueKey == null && matchingNodes.Count() > 1)
                {
                    throw new ApplicationException($"Cannot merge into file containing multiple undelimited equal siblings, needs attribute with name: {_options.UniqueAttributes.FriendlyCommaSeparated("or")}. The offender is: " + input);
                }
                if (uniqueKey == null && matchingNodes.Count() == 1)
                {
                    var templateNode = templateElements.First(x => x.Name == dataNode.Name);
                    Merge(templateNode, dataNode);
                    continue;
                }

                if (uniqueKey != null)
                {
                    var ambigous =
                        templateElements.Count(
                            x =>
                            x.Attribute(uniqueKey.Key) != null &&
                            string.Equals(x.Attribute(uniqueKey.Key).Value, uniqueKey.Value, StringComparison.CurrentCultureIgnoreCase)) > 1;
                    if (ambigous)
                    {
                        throw new ApplicationException($"Cannot merge into file containing multiple equal siblings with same identifier, needs unique value for attribute with name: {_options.UniqueAttributes.FriendlyCommaSeparated("or")}. The offender is: " + input);
                    }
                    var templateNode = (from x in templateElements
                                        where
                                            x.Attribute(uniqueKey.Key) != null
                                            && string.Equals(x.Attribute(uniqueKey.Key).Value, uniqueKey.Value, StringComparison.CurrentCultureIgnoreCase)
                                        select x).FirstOrDefault();
                    if (templateNode == null && !shouldDeleteDataNode)
                    {
                        if (_options.EnableTrace)
                        {
                            dataNode.SetAttributeValue("TRACE", TraceText("CREATED"));
                        }
                        Log.Trace($"{_source.FilePath}: Adding {dataNode}");
                        template.Add(dataNode);
                    }
                    else
                    {
                        Merge(templateNode, dataNode);
                    }
                }
            }
        }

        private KeyValue GetUniqueKey(XElement element)
        {
            foreach (var key in _options.UniqueAttributes)
            {
                var elementsUnique = element.Attributes(key).ToArray();
                if (elementsUnique.Length == 1)
                {
                    return new KeyValue(key, elementsUnique.Single().Value);
                }
            }
            return null;
        }

        private void MergeAttributes(XElement template, XElement data)
        {
            foreach (var attribute in data.Attributes())
            {
                var value = attribute.Value == _options.DeleteKeyword ? null : attribute.Value;

                if (_options.EnableTrace && !_options.UniqueAttributes.Contains(attribute.Name.LocalName))
                {
                    template.SetAttributeValue("TRACE", TraceText("CHANGED"));
                }
                template.SetAttributeValue(attribute.Name, value);
            }
        }

        private bool ShouldDeleteNode(XElement data)
        {
            var deleteattrib = data.Attributes(_options.DeleteKeyword).ToArray();
            return deleteattrib.Length == 1 && bool.Parse(deleteattrib.First().Value);
        }

        private string TraceText(string operation)
        {
            return _options.EnableTrace
                ? $"[{operation} BY {_source.FilePath}]"
                : null;
        }
    }
}