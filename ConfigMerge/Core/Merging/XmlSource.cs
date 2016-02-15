using System.Xml.Linq;

namespace ConfigMerge.Core.Merging
{
    public class XmlSource
    {
        public string FilePath { get; private set; }
        public bool Exists => Content != null;
        public XDocument Content { get; }

        public XmlSource(string filePath, XDocument content)
        {
            FilePath = filePath;
            Content = content;
        }

        public static XmlSource NonExisting(string filePath)
        {
            return new XmlSource(filePath, null);
        }
    }
}