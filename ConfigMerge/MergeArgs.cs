using ConfigMerge.ConsoleArguments;
using ConfigMerge.Logging;

namespace ConfigMerge
{
    public class MergeArgs
    {
        [Argument(Required = true, Description = "recipe; file or argument")]
        public string Recipe { get; set; }
        [Argument(Required = false, Description = "loglevel")]
        public LogLevel L { get; set; }

        public MergeArgs()
        {
            L = LogLevel.Normal;
        }
    }
}