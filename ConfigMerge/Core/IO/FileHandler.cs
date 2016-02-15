using System;
using System.IO;
using System.Xml.Linq;
using ConfigMerge.Core.Merging;

namespace ConfigMerge.Core.IO
{
    public class FileHandler
    {
        public XmlSource ReadXml(string filePath)
        {
            var source = ReadXmlIfExists(filePath);
            if (!source.Exists)
            {
                throw new ApplicationException($"{filePath} does not exist");
            }
            return source;
        }

        public XmlSource ReadXmlIfExists(string filePath)
        {

            var file = TryGetFileInfo(filePath);
            if (!file.Exists)
            {
                return XmlSource.NonExisting(filePath);
            }
            using (var stream = file.OpenRead())
            {
                return new XmlSource(file.FullName, XDocument.Load(stream));
            }
        }

        private FileInfo TryGetFileInfo(string filePath)
        {
            try
            {
                return new FileInfo(filePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Could not read {filePath}", ex);
            }
        }

        public string Read(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ApplicationException($"{filePath} does not exist");
            }
            return File.ReadAllText(filePath);
        }

        public void WriteXml(string filePath, XDocument content)
        {
            File.WriteAllText(filePath, content.ToString());
        }
    }
}