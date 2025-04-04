using QuickAcid.Reporting;

namespace QuickAcid.Tests
{
    public class ActAndSpecExceptionTests
    {
        [Fact]
        public void ExceptionThrownByAct()
        {
            var run =
                from foo in "foo".Act(() => { if (true) throw new Exception(); })
                from spec in "spec".Spec(() => true)
                select Acid.Test;
            var report = run.ReportIfFailed();
            var entry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(entry);
            Assert.NotNull(entry.Exception);
            Assert.Equal("foo", entry.Key);
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

        // needs to be here because of contains assert above
        private bool Throw()
        {
            throw new Exception();
        }
    }
}