using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using ConfigMerge.ConsoleArguments;
using ConfigMerge.Core;
using ConfigMerge.Core.Lang;
using ConfigMerge.Core.Merging;
using ConfigMerge.Logging;
using ConfigMerge.Options;

namespace ConfigMerge
{
    internal class Program
    {
        private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;

        private static string Greeting => $"{ProcessName} v1.0";

        private static int Main(string[] args)
        {
            Console.WriteLine(Greeting);
            if (args.NeedsHelp())
            {
                PrintUsage();
                return ErrorCodes.MissingUserInput;
            }
            try
            {
                var arguments = args.To<MergeArgs>();
                Logger.Level = arguments.L;
                var source = RecipeSource.FromFileOrInput(arguments.Recipe);
                var recipe = new RecipeParser(source).Recipe;
                var options = TransformOptionsProvider.GetTransformOptions();
                recipe.Compile()(new ConfigTransformer(source.BasePath, options));
                return ErrorCodes.Ok;
            }
            catch (RecipeCompilerException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return ErrorCodes.RecipeError;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ProcessName}: error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                return ErrorCodes.Unhandled;
            }
        }

        private static void PrintUsage()
        {
            var usage = new StringBuilder("Usage:").AppendLine()
                .Append($"{ProcessName} ").AppendLine(Args.GetUsage<MergeArgs>());
            Console.WriteLine(usage);
        }
    }

    internal static class ErrorCodes
    {
        public const int Ok = 0;
        public const int MissingUserInput = 10;
        public const int RecipeError = 20;
        public const int Unhandled = 100;
    }
}
