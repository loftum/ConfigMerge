namespace ConfigMerge.Core.Merging
{
    public class KeyValue
    {
        public string Key { get; }
        public string Value { get; }

        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}