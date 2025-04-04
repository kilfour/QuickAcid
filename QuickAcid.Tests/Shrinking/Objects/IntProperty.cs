﻿using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Tests.Shrinking.Objects
{
    public class IntProperty
    {
        [Fact]
        public void OneRelevantProperty()
        {
            var generator =
                from _ in MGen.Int(5, 7).Replace()
                from thing in MGen.One<Something>()
                select thing;
            var run =
                from input in "input".ShrinkableInput(generator)
                from foo in "act".Act(() =>
                {
                    if (input.MyFirstProperty == 6) throw new Exception();
                })
                select Acid.Test;

            var report = run.ReportIfFailed(1, 50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
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
                from _ in MGen.Int(5, 7).Replace()
                from thing in MGen.One<Something>()
                select thing;
            var run =

                    from input in "input".ShrinkableInput(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.MyFirstProperty == 6 && input.MySecondProperty == 6) throw new Exception();
                    })
                    select Acid.Test;

            var report = run.ReportIfFailed(1, 50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
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
                from _ in MGen.Int(5, 7).Replace()
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
                            if (input.MyFirstProperty == 6 || input.MySecondProperty == 6) throw new Exception();
                        })
                        select Acid.Test;

                var report = run.ReportIfFailed(1, 50);

                var inputEntry = report.FirstOrDefault<ReportInputEntry>();
                Assert.NotNull(inputEntry);
                Assert.NotNull(inputEntry.Value);
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
                from _ in MGen.Constant(42).Replace()
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


            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("MySecondProperty : 42, MyThirdProperty : 42", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.Null(actEntry.Exception);

            var specEntry = report.Single<QAcidReportSpecEntry>();
            Assert.Equal("equal", specEntry.Key);
        }

        [Fact]
        public void TwoRelevantPropertiesSuperTricky()
        {
            var generator =
                from _ in MGen.Constant(42).Replace()
                from cust in MGen.For<Something>().Customize(x => x.MyFifthProperty, 0)
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

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("MySecondProperty : 42, MyThirdProperty : 42", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.Null(actEntry.Exception);

            var specEntry = report.Single<QAcidReportSpecEntry>();
            Assert.Equal("equal", specEntry.Key);
        }

        [Fact]
        public void ThreeRelevantProperties()
        {
            var generator =
                from _ in MGen.Constant(42).Replace()
                from cust in MGen.For<Something>().Customize(x => x.MyFifthProperty, 0)
                from thing in MGen.One<Something>()
                select thing;

            var run =

                    from input in "input".ShrinkableInput(generator)
                    from result in "act".Act(() => new Something { MyFirstProperty = input.MyFirstProperty })
                    from spec in "equal".Spec(() =>
                        input.MyFirstProperty == result.MyFirstProperty
                        && input.MySecondProperty == result.MySecondProperty
                        && input.MyThirdProperty == result.MyThirdProperty
                        && input.MyFourthProperty == result.MyFourthProperty)
                    select Acid.Test;

            var report = run.ReportIfFailed(1, 50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("MySecondProperty : 42, MyThirdProperty : 42, MyFourthProperty : 42", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.Null(actEntry.Exception);

            var specEntry = report.Single<QAcidReportSpecEntry>();
            Assert.Equal("equal", specEntry.Key);
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
}