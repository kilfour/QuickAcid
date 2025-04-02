using QuickAcid.Tests.ZheZhools;

namespace QuickAcid.Tests
{
    public class ActAndSpecExceptionTests
    {
        [Fact]
        public void ExceptionThrownByAct()
        {
            var run =
                AcidTestRun.FailedRun(
                    from foo in "foo".Act(() => { if (true) throw new Exception(); })
                    from spec in "spec".Spec(() => true)
                    select Acid.Test);

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<QAcidReportActEntry>(0);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void ExceptionThrownBySpecIsNotAQuickAcidException()
        {
            var ex =
                Assert.Throws<Exception>(() => (
                    from foo in "foo".Act(() => true)
                    from spec in "spec".Spec(Throw)
                    select Acid.Test
                ).Verify(1, 1));
            Assert.IsNotType<FalsifiableException>(ex);
            Assert.Contains("QuickAcid.Tests.ActAndSpecExceptionTests.Throw()", ex.StackTrace);
        }

        private bool Throw()
        {
            throw new Exception();
        }
    }
}