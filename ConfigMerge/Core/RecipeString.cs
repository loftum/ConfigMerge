using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConfigMerge.Core.Lang;

namespace ConfigMerge.Core
{
    public class RecipeString : IRecipeSource
    {
        private readonly string _recipe;

        public string BasePath { get; }
        public string FullPath => Name;
        public string Name => "input";
        
        public RecipeString(string recipe)
        {
            _recipe = recipe;
            BasePath = Directory.GetCurrentDirectory();
        }

        public Stream GetStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_recipe));
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return new RecipeLexer(new SourceStreamEnumerator(this));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static RecipeString FromLines(IEnumerable<string> lines)
        {
            return new RecipeString(string.Join(Environment.NewLine, lines));
        }

        public static implicit operator RecipeString(string value)
        {
            return new RecipeString(value);
        }

        public static implicit operator RecipeString(string[] lines)
        {
            return FromLines(lines);
        }
    }
}