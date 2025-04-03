using QuickAcid.Tests.ZheZhools;

namespace QuickAcid.Tests
{
    public class ActExceptionTests
    {
        [Fact]
        public void SimpleExceptionThrown()
        {
            var run =
                AcidTestRun.FailedRun("foo".Act(() => { if (true) throw new Exception(); }));

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<QAcidReportActEntry>(0);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void TwoActionsExceptionThrownOnFirst()
        {
            var run =
                AcidTestRun.FailedRun(
                    from foo in "foo".Act(() => throw new Exception())
                    from bar in "bar".Act(() => { })
                    select Acid.Test);

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<QAcidReportActEntry>(0);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void TwoActionsExceptionThrownOnSecond()
        {
            var run =
                AcidTestRun.FailedRun(
                    from foo in "foo".Act(() => { })
                    from bar in "bar".Act(() => throw new Exception())
                    select Acid.Test);

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<QAcidReportActEntry>(0);
            Assert.Equal("bar", entry.Key);
            Assert.NotNull(entry.Exception);
        }
    }
}
