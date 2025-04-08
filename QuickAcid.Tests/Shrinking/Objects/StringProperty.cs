using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Nuts;
using QuickAcid.Nuts.Bolts;

namespace QuickAcid.Tests.Shrinking.Objects
{
    public class StringProperty
    {
        [Fact]
        public void OneRelevantProperty()
        {
            var generator =
                from _ in MGen.Int(5, 7).AsString().Replace()
                from thing in MGen.One<Something>()
                select thing;
            var run =

                    from input in "input".ShrinkableInput(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.MyFirstProperty == "6") throw new Exception();
                    })
                    select Acid.Test;

            var report = run.ReportIfFailed(1, 50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>(); ;
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("MyFirstProperty : 6", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }

        [Fact]
        public void TwoRelevantProperties()
        {
            var generator =
                from _ in MGen.Int(5, 7).AsString().Replace()
                from thing in MGen.One<Something>()
                select thing;
            var run =

                    from input in "input".ShrinkableInput(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.MyFirstProperty == "6" && input.MySecondProperty == "6") throw new Exception();
                    })
                    select Acid.Test;

            var report = run.ReportIfFailed(1, 50);


            var inputEntry = report.FirstOrDefault<ReportInputEntry>(); ;
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("MyFirstProperty : 6, MySecondProperty : 6", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }

        [Fact]
        public void TwoRelevantPropertiesTricky()
        {
            var generator =
                from _ in MGen.Int(5, 7).AsString().Replace()
                from thing in MGen.One<Something>()
                select thing;

            var sometimesPropOne = false;
            var sometimesPropTwo = false;
            var sometimesBothProps = false;
            var somethingElse = "";
            for (int i = 0; i < 100; i++)
            {
                var run =

                        from input in "input".ShrinkableInput(generator)
                        from foo in "act".Act(() =>
                        {
                            if (input.MyFirstProperty == "6" || input.MySecondProperty == "6") throw new Exception();
                        })
                        select Acid.Test;

                var report = run.ReportIfFailed(1, 50);

                var inputEntry = report.FirstOrDefault<ReportInputEntry>(); ;
                Assert.Equal("input", inputEntry.Key);
                if ((string)inputEntry.Value == "MyFirstProperty : 6")
                    sometimesPropOne = true;
                else if ((string)inputEntry.Value == "MySecondProperty : 6")
                    sometimesPropTwo = true;
                else if ((string)inputEntry.Value == "MyFirstProperty : 6, MySecondProperty : 6")
                    sometimesBothProps = true;
                else
                    somethingElse = (string)inputEntry.Value;

                var actEntry = report.FirstOrDefault<ReportActEntry>();
                Assert.NotNull(actEntry);
                Assert.Equal("act", actEntry.Key);
                Assert.NotNull(actEntry.Exception);
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
                from _ in MGen.Constant(42).AsString().Replace()
                from cust in MGen.For<Something>().Customize(x => x.MyFourthProperty, "")
                from thing in MGen.One<Something>()
                select thing;

            var run =

                    from input in "input".ShrinkableInput(generator)
                    from result in "act".Act(() => new Something { MyFirstProperty = input.MyFirstProperty })
                    from spec in "equal".Spec(() =>
                        input.MyFirstProperty == result.MyFirstProperty
                        && input.MySecondProperty == result.MySecondProperty
                        && input.MyThirdProperty == result.MyThirdProperty)
                    select Acid.Test;

            var report = run.ReportIfFailed(1, 50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>(); ;
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("MySecondProperty : 42, MyThirdProperty : 42", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.Null(actEntry.Exception);

            var specEntry = report.GetSpecEntry();
            Assert.Equal("equal", specEntry.Key);
        }


        public class Something
        {
            public string MyFirstProperty { get; set; }
            public string MySecondProperty { get; set; }
            public string MyThirdProperty { get; set; }
            public string MyFourthProperty { get; set; }
        }
    }
}