using QuickAcid;
using QuickAcid.Bolts.ShrinkStrats.Objects;
using QuickAcid.Reporting;
using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;

namespace QuickAcid.Tests.Shrinking.Objects;

public class IntProperty
{
    public class Simples
    {
        public int AnInt { get; set; }
    }

    [Fact]
    public void SimplesRelevant()
    {
        var generator =
            from _ in Fuzz.Constant(6).Replace()
            from thing in Fuzz.One<Simples>()
            select thing;
        var script =
            from input in "input".Input(generator)
            from foo in "act".Act(() =>
            {
                if (input.AnInt == 6) throw new Exception();
            })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);
        Assert.Equal("{ AnInt : 6 }", inputEntry.Value);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void SimplesIrrelevant()
    {
        var generator =
            from _ in Fuzz.Int(5, 7).Replace()
            from thing in Fuzz.One<Simples>()
            select thing;
        var script =
            from input in "input".Input(generator)
            from foo in "act".Act(() => { throw new Exception(); })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        Assert.Equal(0, article.Total().Inputs());

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void OneRelevantProperty()
    {
        var generator =
            from _ in Fuzz.Int(5, 7).Replace()
            from thing in Fuzz.One<Something>()
            select thing;
        var script =
            from input in "input".Input(generator)
            from foo in "act".Act(() =>
            {
                if (input.MyFirstProperty == 6) throw new Exception();
            })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));
        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);
        Assert.Equal("{ MyFirstProperty : 6 }", inputEntry.Value);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void TwoRelevantProperties()
    {
        var generator =
            from _ in Fuzz.Int(5, 7).Replace()
            from thing in Fuzz.One<Something>()
            select thing;
        var script =

                from input in "input".Input(generator)
                from foo in "act".Act(() =>
                {
                    if (input.MyFirstProperty == 6 && input.MySecondProperty == 6) throw new Exception();
                })
                select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);
        Assert.Equal("{ MyFirstProperty : 6, MySecondProperty : 6 }", inputEntry.Value);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void TwoRelevantPropertiesTricky()
    {
        var generator =
            from _ in Fuzz.Int(5, 7).Replace()
            from thing in Fuzz.One<Something>()
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
                    if (input.MyFirstProperty == 6 || input.MySecondProperty == 6) throw new Exception();
                })
                select Acid.Test;

            var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));
            var inputEntry = article.Execution(1).Input(1).Read();
            Assert.NotNull(inputEntry);
            Assert.NotNull(inputEntry.Value);
            Assert.Equal("input", inputEntry.Label);
            if ((string)inputEntry.Value == "{ MyFirstProperty : 6 }")
                sometimesPropOne = true;
            else if ((string)inputEntry.Value == "{ MySecondProperty : 6 }")
                sometimesPropTwo = true;
            else if ((string)inputEntry.Value == "{ MyFirstProperty : 6, MySecondProperty : 6 }")
                sometimesBothProps = true;
            else
                somethingElse = (string)inputEntry.Value;

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


    [Fact]
    public void TwoRelevantPropertiesEvenTrickier()
    {
        var generator =
            from _ in Fuzz.Constant(42).Replace()
            from thing in Fuzz.One<Something>()
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

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));
        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);
        Assert.Equal("{ MySecondProperty : 42, MyThirdProperty : 42 }", inputEntry.Value);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.Null(article.Exception());

        Assert.Equal("equal", article.FailedSpec());
    }

    [Fact]
    public void TwoRelevantPropertiesSuperTricky()
    {
        var generator =
            from _ in Fuzz.Constant(42).Replace()
            from cust in Fuzz.For<Something>().Customize(x => x.MyFifthProperty, 0)
            from thing in Fuzz.One<Something>()
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

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);
        Assert.Equal("{ MySecondProperty : 42, MyThirdProperty : 42 }", inputEntry.Value);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.Null(article.Exception());

        Assert.Equal("equal", article.FailedSpec());
    }

    [Fact]
    public void ThreeRelevantProperties()
    {
        var generator =
            from _ in Fuzz.Constant(42).Replace()
            from cust in Fuzz.For<Something>().Customize(x => x.MyFifthProperty, 0)
            from thing in Fuzz.One<Something>()
            select thing;

        var script =
            from _ in ShrinkingPolicy.ForObjects(new ObjectShrinkStrategy(), new PowersetPropertyNullingStrategy())
            from input in "input".Input(generator)
            from result in "act".Act(() => new Something { MyFirstProperty = input.MyFirstProperty })
            from spec in "equal".Spec(() =>
                input.MyFirstProperty == result.MyFirstProperty
                && input.MySecondProperty == result.MySecondProperty
                && input.MyThirdProperty == result.MyThirdProperty
                && input.MyFourthProperty == result.MyFourthProperty)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);
        Assert.Equal("{ MySecondProperty : 42, MyThirdProperty : 42, MyFourthProperty : 42 }", inputEntry.Value);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.Null(article.Exception());

        Assert.Equal("equal", article.FailedSpec());
    }


    public class Something
    {
        public int MyFirstProperty { get; set; }
        public int MySecondProperty { get; set; }
        public int MyThirdProperty { get; set; }
        public int MyFourthProperty { get; set; }
        public int MyFifthProperty { get; set; }
    }
}