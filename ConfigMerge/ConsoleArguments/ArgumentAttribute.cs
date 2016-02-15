using System;

namespace ConfigMerge.ConsoleArguments
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ArgumentAttribute : Attribute
    {
        public bool Required { get; set; }
        public string Description { get; set; }
    }
}