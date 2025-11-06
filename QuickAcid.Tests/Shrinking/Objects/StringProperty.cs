using QuickFuzzr;
using QuickAcid.Bolts.ShrinkStrats.Objects;
using QuickAcid;
using QuickAcid.Tests._Tools.ThePress;
using QuickAcid.Shrinking.Objects;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Shrinking.Objects
{
    public class StringProperty
    {
        [Fact]
        public void OneRelevantProperty()
        {
            var generator =
                from _ in Configr.Primitive(Fuzzr.OneOf("5", "6"))
                from thing in Fuzzr.One<Something>()
                select thing;
            var script =

                    from input in "input".Input(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.MyFirstProperty == "6") throw new Exception();
                    })
                    select Acid.Test;

            var article = TheJournalist.Exposes(() => QState.Run(script)
                .WithOneRun()
                .And(50.ExecutionsPerRun()));

            var inputEntry = article.Execution(1).Input(1).Read();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Label);
            Assert.Equal("{ MyFirstProperty : \"6\" }", inputEntry.Value);

            var actEntry = article.Execution(1).Action(1).Read();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Label);
            Assert.NotNull(article.Exception());
        }

        [Fact]
        public void TwoRelevantProperties()
        {
            var generator =
                from _ in Configr.Primitive(Fuzzr.OneOf("5", "6"))
                from thing in Fuzzr.One<Something>()
                select thing;
            var script =

                    from input in "input".Input(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.MyFirstProperty == "6" && input.MySecondProperty == "6") throw new Exception();
                    })
                    select Acid.Test;

            var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

            var inputEntry = article.Execution(1).Input(1).Read();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Label);
            Assert.Equal("{ MyFirstProperty : \"6\", MySecondProperty : \"6\" }", inputEntry.Value);

            var actEntry = article.Execution(1).Action(1).Read();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Label);
            Assert.NotNull(article.Exception());
        }

        [Fact]
        public void TwoRelevantPropertiesTricky()
        {
            var generator =
                from _ in Configr.Primitive(Fuzzr.OneOf("5", "6"))
                from thing in Fuzzr.One<Something>()
                select thing;

            var sometimesPropOne = false;
            var sometimesPropTwo = false;
            var sometimesBothProps = false;
            var somethingElse = "";
            for (int i = 0; i < 100; i++)
            {
                var script =
                    from _ in ShrinkingPolicy.ForObjects(new ObjectShrinkStrategy(), new PowersetPropertyNullingStrategy())
                    from input in "input".Input(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.MyFirstProperty == "6" || input.MySecondProperty == "6") throw new Exception();
                    })
                    select Acid.Test;

                var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

                var inputEntry = article.Execution(1).Input(1).Read();
                Assert.NotNull(inputEntry);
                Assert.Equal("input", inputEntry.Label);
                if ((string)inputEntry.Value! == "{ MyFirstProperty : \"6\" }")
                    sometimesPropOne = true;
                else if ((string)inputEntry.Value! == "{ MySecondProperty : \"6\" }")
                    sometimesPropTwo = true;
                else if ((string)inputEntry.Value! == "{ MyFirstProperty : \"6\", MySecondProperty : \"6\" }")
                    sometimesBothProps = true;
                else
                    somethingElse = (string)inputEntry.Value!;

                var actEntry = article.Execution(1).Action(1).Read();
                Assert.NotNull(actEntry);
                Assert.Equal("act", actEntry.Label);
                Assert.NotNull(article.Exception());
            }
            Assert.Equal("", somethingElse);
            Assert.True(sometimesPropOne);
            Assert.True(sometimesPropTwo);
            Assert.True(sometimesBothProps);
        }


        [Fact(Skip = "String fuzzr change messed up quickacid")]
        public void TwoRelevantPropertiesEvenTrickier()
        {
            var generator =
                from _ in Configr.Primitive(Fuzzr.Constant("42"))
                from cust in Configr<Something>.Property(x => x.MyFourthProperty, "")
                from thing in Fuzzr.One<Something>()
                select thing;

            var script =
                from _ in ShrinkingPolicy.ForObjects(new ObjectShrinkStrategy(), new PowersetPropertyNullingStrategy())
                from input in "input".Input(generator)
                from result in "act".Act(() => new Something { MyFirstProperty = input.MyFirstProperty })
                from spec in "equal".Spec(() =>
                    input.MyFirstProperty == result.MyFirstProperty
                    && input.MySecondProperty == result.MySecondProperty
                    && input.MyThirdProperty == result.MyThirdProperty)
                select Acid.Test;

            var article = TheJournalist.Exposes(() => QState.Run("temp", script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

            var inputEntry = article.Execution(1).Input(1).Read();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Label);
            Assert.Equal("{ MySecondProperty : \"42\", MyThirdProperty : \"42\" }", inputEntry.Value);

            var actEntry = article.Execution(1).Action(1).Read();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Label);
            Assert.Null(article.Exception());

            Assert.Equal("equal", article.FailedSpec());
        }


        public class Something
        {
            public string MyFirstProperty { get; set; } = string.Empty;
            public string MySecondProperty { get; set; } = string.Empty;
            public string MyThirdProperty { get; set; } = string.Empty;
            public string MyFourthProperty { get; set; } = string.Empty;
        }
    }
}