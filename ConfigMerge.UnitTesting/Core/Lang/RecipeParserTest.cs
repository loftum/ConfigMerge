using System;
using System.Collections.Generic;
using System.Linq;
using ConfigMerge.Core;
using ConfigMerge.Core.Lang;
using FakeItEasy;
using NUnit.Framework;

namespace ConfigMerge.UnitTesting.Core.Lang
{
    [TestFixture]
    public class RecipeParserTest
    {
        private IConfigTransformer _transformer;

        [SetUp]
        public void Setup()
        {
            _transformer = A.Fake<IConfigTransformer>();
        }

        [Test]
        public void EmptyScript_ReturnsVoidExpression()
        {
            var expression = new RecipeParser(Enumerable.Empty<Token>()).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform(A<string>._, A<IEnumerable<string>>._)).MustNotHaveHappened();
        }

        [Test]
        public void Simple()
        {
            RecipeString source = "App.config = [App.root.config, App.dev.local.config];";
            var expression = new RecipeParser(source).Recipe;

            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new [] { "App.root.config", "App.dev.local.config"})))
                .MustHaveHappened();
        }

        [Test]
        public void WithComments()
        {
            RecipeString source = new[]
            {
                "#comment",
                "App.config = [App.root.config, App.dev.local.config];"
            };
            
            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void EmptyLines()
        {
            RecipeString source = new[]
            {
                "",
                "#comment",
                "",
                "App.config = [App.root.config, App.dev.local.config];",
                ""
            };
            
            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void MultipleLines()
        {
            RecipeString source = new[]
            {
                "App.config = ",
                "[",
                "App.root.config,",
                "App.dev.local.config",
                "]"
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void ArgumentsAsString()
        {
            RecipeString source = new[]
            {
                "\"App.config\" = [\"App.root.config\", \"App.dev.local.config\"];"
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void Variable()
        {
            RecipeString source = new[]
            {
                "",
                "var root = App.root.config;",
                "",
                "App.config = [root, App.dev.local.config];",
                ""
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void ExpressionInVariable()
        {
            RecipeString source = new[]
            {
                "",
                "var root = \"App\" + \".root.config\";",
                "",
                "App.config = [root, App.dev.local.config];",
                ""
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void ExpressionInMerge()
        {
            RecipeString source = new[]
            {
                "",
                "var c = \"config/\";",
                "",
                "App.config = [c + App.root.config, c + App.dev.local.config];",
                ""
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "config/App.root.config", "config/App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void ExpressionInOutput()
        {
            RecipeString source = new[]
            {
                "App + .config = [App.root.config, App.dev.local.config];"
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void ArrayManipulations()
        {
            RecipeString source = new[]
            {
                "App.config = App. + [root, dev.local] + .config;"
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void AddTwoArrays()
        {
            RecipeString source = new[]
            {
                "App.config = [App.root.config, App.dev.local.config] + [another.file];"
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config", "App.dev.local.config", "another.file" })))
                .MustHaveHappened();
        }

        [Test]
        public void SingleFile()
        {
            RecipeString source = new[]
            {
                "App.config = App.root.config;"
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "App.root.config" })))
                .MustHaveHappened();
        }

        [Test]
        public void CrazyArrayManiuplations()
        {
            RecipeString source = new[]
            {
                "var i = \"input/\";",
                "App.config = i + (App. + [root, dev.local] + .config) + [another.file];"
            };

            var expression = new RecipeParser(source).Recipe;
            expression.Compile()(_transformer);
            A.CallTo(() => _transformer.Transform("App.config", A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "input/App.root.config", "input/App.dev.local.config", "input/another.file" })))
                .MustHaveHappened();
        }
    }
}