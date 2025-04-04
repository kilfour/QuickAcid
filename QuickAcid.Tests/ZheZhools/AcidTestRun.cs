﻿using QuickAcid.Reporting;

namespace QuickAcid.Tests.ZheZhools
{
    public class AcidTestRun
    {
        private readonly QAcidReport report;

        public AcidTestRun(QAcidReport report)
        {
            this.report = report;
        }


        public T GetReportEntryAtIndex<T>(int index)
        {
            return Assert.IsType<T>(report.Entries[index]);
        }

        public static AcidTestRun FailedRun(QAcidRunner<Acid> test)
        {
            var ex = Assert.Throws<FalsifiableException>(() => test.Verify(1, 1));
            return new AcidTestRun(ex.QAcidReport);
        }

        public static AcidTestRun FailedRun(int numberOfActions, QAcidRunner<Acid> test)
        {
            var ex = Assert.Throws<FalsifiableException>(() => test.Verify(1, numberOfActions));
            return new AcidTestRun(ex.QAcidReport);
        }
    }
}