using System;
using System.Linq;
using ConfigMerge.Common;
using ConfigMerge.Logging;
using static System.Configuration.ConfigurationManager;

namespace ConfigMerge.Options
{
    public class OverrideTransformOptions
    {
        private static readonly ILogger Log = Logger.For<OverrideTransformOptions>();

        public bool? EnableTrace { get; set; }
        public string DeleteKeyword { get; set; }
        public string[] UniqueAttributes { get; set; }

        public static OverrideTransformOptions FromConfig()
        {
            return new OverrideTransformOptions
            {
                EnableTrace = ParseSetting("EnableTrace", b => (bool?) bool.Parse(b)),
                DeleteKeyword = GetAppsetting("DeleteKeyword"),
                UniqueAttributes = ParseSetting("UniqueAttributes", s => s.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToArray())
            };
        }

        private static string GetAppsetting(string name)
        {
            var value = AppSettings[name];
            if (value == null)
            {
                return null;
            }
            Log.Debug($"Override from config: {name}='{value}'");
            return value;
        }

        private static T ParseSetting<T>(string name, Func<string, T> convert, T defaultValue = default(T))
        {
            var value = AppSettings[name];
            if (value == null)
            {
                return default(T);
            }
            try
            {
                var ret = convert(value);
                Log.Debug($"Override from config: {name}='{ret.Friendly()}'");
                return ret;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}