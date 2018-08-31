using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests.ZheZhools
{
    public class AcidTestRun
    {
        private readonly QAcidReport report;

        public AcidTestRun(QAcidReport report)
        {
            this.report = report;
        }

        public void NumberOfReportEntriesIs(int count)
        {
            Assert.Equal(count, report.Entries.Count);
        }

        public T GetReportEntryAtIndex<T>(int index)
        {
            return Assert.IsType<T>(report.Entries[index]);
        }

        public static AcidTestRun FailedRun(QAcidRunner<Unit> test)
        {
            var ex = Assert.Throws<FalsifiableException>(() => test.Verify(1, 1));
            return new AcidTestRun(ex.QAcidReport);
        }

        public static AcidTestRun FailedRun(int numberOfActions, QAcidRunner<Unit> test)
        {
            var ex = Assert.Throws<FalsifiableException>(() => test.Verify(1, numberOfActions));
            return new AcidTestRun(ex.QAcidReport);
        }

        public static void SuccessfullRun(QAcidRunner<Unit> test)
        {
            test.Verify(1, 1);
        }
    }
}