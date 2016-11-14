using System;

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

        public override string ToString()
        {
            return $"{Key}={Value}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyValue);
        }

        public bool Equals(KeyValue other)
        {
            return other != null &&
                   string.Equals(Key, other.Key, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key?.ToLowerInvariant().GetHashCode() ?? 0) * 397) ^ (Value?.ToLowerInvariant().GetHashCode() ?? 0);
            }
        }
    }
}