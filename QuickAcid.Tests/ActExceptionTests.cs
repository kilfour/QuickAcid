using System;
using QuickAcid.Tests.ZheZhools;
using QuickMGenerate.UnderTheHood;
using Xunit;

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

            var entry = run.GetReportEntryAtIndex<AcidReportActEntry>(0);
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
                    select Unit.Instance);

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<AcidReportActEntry>(0);
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
                    select Unit.Instance);


            // -------------------------------------------------------------------------
            run.NumberOfReportEntriesIs(2);
            //               _____ 
            //   _ _ _ _    |___  |
            //  | | | | |_ _ _|  _|
            //  | | | |   | | |_|  
            //  |_____|_|_|_  |_|  
            //            |___|    
            //
            // Using 2 Act() thingies in one linq expression does not shrink the actions
            // Use either Sequence() or Choose() to combine them
            // -------------------------------------------------------------------------

            var entry = run.GetReportEntryAtIndex<AcidReportActEntry>(0);
            Assert.Equal("foo", entry.Key);
            Assert.Null(entry.Exception);

            entry = run.GetReportEntryAtIndex<AcidReportActEntry>(1);
            Assert.Equal("bar", entry.Key);
            Assert.NotNull(entry.Exception);
        }
    }
}
