using System.Linq;
using System.Xml.Linq;
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
            var meta = GetMeta(input);
            if (meta.ShouldDelete)
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
            MergeChildren(template, input);
        }

        private void MergeChildren(XElement template, XElement input)
        {
            var inputElements = input.Elements();
            var templateElements = template.Elements().ToArray();
            foreach (var inputElement in inputElements)
            {
                var meta = GetMeta(inputElement);
                var match = meta.GetMatch(templateElements);

                if (match == null)
                {
                    if (meta.ShouldDelete)
                    {
                        continue;
                    }
                    Log.Trace($"{_source.FilePath}: Adding {inputElement}");
                    if (_options.EnableTrace)
                    {
                        inputElement.SetAttributeValue("TRACE", TraceText("CREATED"));
                    }
                    template.Add(inputElement);
                }
                else
                {
                    Merge(match, inputElement);
                }
            }
        }

        private ElementMeta GetMeta(XElement input)
        {
            return new ElementMeta(input, _options);
        }

        private void MergeAttributes(XElement template, XElement input)
        {
            foreach (var attribute in input.Attributes())
            {
                var value = attribute.Value == _options.DeleteKeyword ? null : attribute.Value;

                if (_options.EnableTrace && !_options.UniqueAttributes.Contains(attribute.Name.LocalName))
                {
                    template.SetAttributeValue("TRACE", TraceText("CHANGED"));
                }
                template.SetAttributeValue(attribute.Name, value);
            }
        }

        private string TraceText(string operation)
        {
            return _options.EnableTrace
                ? $"[{operation} BY {_source.FilePath}]"
                : null;
        }
    }
}