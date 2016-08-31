using System.Diagnostics;

namespace ConfigMerge.Meta
{
    public static class ProgramInfo
    {
        public static readonly string Name;
        public static readonly string Version;
        public static string Greeting => $"{Name} {Version}";

        static ProgramInfo()
        {
            Name = Process.GetCurrentProcess().ProcessName;
            Version = typeof (ProgramInfo).Assembly.GetName().Version.ToString();
        }
    }
}