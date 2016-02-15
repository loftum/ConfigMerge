using System.IO;
using ConfigMerge.Core;
using ConfigMerge.Core.Lang;

namespace ConfigMerge.Options
{
    public static class RecipeSource
    {
        public static IRecipeSource FromFileOrInput(string input)
        {
            if (File.Exists(input))
            {
                return new RecipeFile(input);
            }
            return new RecipeString(input);
        }
    }
}