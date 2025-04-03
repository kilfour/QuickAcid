using QuickAcid.Tests.ZheZhools;
using QuickMGenerate;

namespace QuickAcid.Tests.Shrinking.Primitives
{
    public class IntTests
    {
        [Fact]
        public void OneRelevantInt()
        {
            var run =
                AcidTestRun.FailedRun(50,
                    from input1 in "input1".ShrinkableInput(MGen.Int(5, 7))
                    from input2 in "input2".ShrinkableInput(MGen.Int(5, 7))
                    from foo in "act".Act(() =>
                    {
                        if (input1 == 6) throw new Exception();
                    })
                    select Acid.Test);

            run.NumberOfReportEntriesIs(2);

            var inputÈntry = run.GetReportEntryAtIndex<QAcidReportInputEntry>(0);
            Assert.Equal("input1", inputÈntry.Key);
            Assert.Equal("6", inputÈntry.Value);

            var actEntry = run.GetReportEntryAtIndex<QAcidReportActEntry>(1);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }

        [Fact]
        public void TwoRelevantInts()
        {
            var run =
                AcidTestRun.FailedRun(50,
                    from input1 in "input1".ShrinkableInput(MGen.Int(5, 7))
                    from input2 in "input2".ShrinkableInput(MGen.Int(5, 7))
                    from foo in "act".Act(() =>
                    {
                        if (input1 == 6 && input2 == 6) throw new Exception();
                    })
                    select Acid.Test);

            run.NumberOfReportEntriesIs(3);

            var inputÈntry = run.GetReportEntryAtIndex<QAcidReportInputEntry>(0);
            Assert.Equal("input1", inputÈntry.Key);
            Assert.Equal("6", inputÈntry.Value);

            inputÈntry = run.GetReportEntryAtIndex<QAcidReportInputEntry>(1);
            Assert.Equal("input2", inputÈntry.Key);
            Assert.Equal("6", inputÈntry.Value);

            var actEntry = run.GetReportEntryAtIndex<QAcidReportActEntry>(2);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }
    }
}