using System;
using QuickAcid.Tests.ZheZhools;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests
{
    public class ActAndSpecExceptionTests
    {
        [Fact]
        public void ExceptionThrownByAct()
        {
            var run =
                AcidTestRun.FailedRun(
                    from foo in "foo".Act(() => { if (true) throw new Exception();})
                    from spec in "spec".Spec(() => true)
                    select Unit.Instance);

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
                    select Unit.Instance
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