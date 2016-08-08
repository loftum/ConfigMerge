namespace ConfigMerge.Core.Merging
{
    public class TransformOptions
    {
        public bool EnableTrace { get; set; }
        public string DeleteKeyword { get; set; }
        public string[] UniqueAttributes { get; set; }

        public TransformOptions()
        {
            DeleteKeyword = "DELETEME";
            UniqueAttributes = new[] { "id", "name", "key", "path", "virtualPath", "href", "namespace", "dependentAssembly/assemblyIdentity/name" };
        }
    }
}