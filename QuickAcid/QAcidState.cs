using QuickAcid.Reporting;

namespace QuickAcid
{
    public class QAcidState
    {
        public QAcidRunner<Acid> Runner { get; private set; }

        public int CurrentActionNumber { get; private set; }

        public List<int> ActionNumbers { get; private set; }

        public Memory Memory { get; private set; }


        public bool Shrinking { get; private set; }
        public Memory Shrunk { get; private set; }
        private int shrinkCount = 0;

        public bool Verifying { get; private set; }


        public bool Failed { get; private set; }
        public string FailingSpec { get; private set; }
        public Exception Exception { get; private set; }

        private QAcidReport report;

        public bool DontThrowFalsifiableException { get; set; }
        public bool Verbose { get; set; }
        public QAcidState(QAcidRunner<Acid> runner)
        {
            Runner = runner;
            ActionNumbers = new List<int>();
            Memory = new Memory(() => CurrentActionNumber);
            Shrunk = new Memory(() => CurrentActionNumber);
        }

        public void Run(int actionsPerRun)
        {
            for (int j = 0; j < actionsPerRun; j++)
            {
                StepAction();
                if (Failed)
                    return;
            }
        }

        public void StepAction()
        {
            ActionNumbers.Add(CurrentActionNumber);
            Runner(this);
            if (Failed)
            {
                if (Verbose)
                {
                    report = new QAcidReport();
                    AddMemoryToReport(report);
                }

                HandleFailure();
                return;
            }
            CurrentActionNumber++;
        }

        public bool IsNormalRun()
        {
            return Verifying == false && Shrinking == false;
        }

        public void FailedWithException(Exception exception)
        {
            if (Verifying)
            {
                if (Exception == null)
                {
                    BreakRun = true;
                    Failed = true;
                    return;
                }

                if (Exception.GetType() != exception.GetType())
                {
                    BreakRun = true;
                    Failed = true;
                    return;
                }
            }
            Failed = true;
            Exception = exception;
        }

        public void SpecFailed(string failingSpec)
        {
            Failed = true;
            FailingSpec = failingSpec;
        }

        private void HandleFailure()
        {
#if DEBUG
            QAcidDebug.WriteLine("BEFORE SHRINKING");
            QAcidDebug.WriteLine(Memory.ToDiagnosticString());
#endif
            ShrinkActions();
            ShrinkInputs();
            if (Verbose)
            {
                Report(report);
            }
            else
                Report();
        }

        public bool BreakRun { get; private set; }

        private void ShrinkActions()
        {
            Verifying = true;
            Shrinking = false;
            BreakRun = false;

            Failed = false;
            var failingSpec = FailingSpec;
            var exception = Exception;

            var max = ActionNumbers.Max();
            var current = 0;

            while (current <= max)
            {
                Failed = false;
                Memory.ResetAllRunInputs();
                FailingSpec = failingSpec;
                Exception = exception;

                foreach (var run in ActionNumbers.ToList())
                {
                    CurrentActionNumber = run;
                    if (run != current)
                        Runner(this);
                    if (BreakRun)
                        break;
                }
                if (Failed && !BreakRun)
                {
                    ActionNumbers.Remove(current);
                }
                current++;
                shrinkCount++;
            }

            Failed = true;
            FailingSpec = failingSpec;
            Exception = exception;
        }

        private void ShrinkInputs()
        {
            Verifying = false;
            Shrinking = true;

            Failed = false;
            Memory.ResetAllRunInputs();

            var failingSpec = FailingSpec;
            var exception = Exception;

            foreach (var run in ActionNumbers.ToList())
            {
                CurrentActionNumber = run;
                Runner(this);
                shrinkCount++;
            }

            Failed = true;
            FailingSpec = failingSpec;
            Exception = exception;
        }

        public bool ShrinkRun(object key, object value) // Only Used by Shrink.cs
        {
            Verifying = true;
            Shrinking = false;
            Failed = false;
            Memory.ResetAllRunInputs();
            var failingSpec = FailingSpec;
            var exception = Exception;
            var runNumber = CurrentActionNumber;
            var oldVal = Memory.ForThisAction().Get<object>(key);
            Memory.ForThisAction().Set(key, value);

#if DEBUG
            var sanityCheck = Memory.ForThisAction().Get<object>(key);
            QAcidDebug.WriteLine("");
            QAcidDebug.WriteLine($"[shrink run] action #{CurrentActionNumber}");
            QAcidDebug.WriteLine($"    {key} was set to {value}, now reads {sanityCheck}");
            QAcidDebug.WriteLine($"    [state] QAcidState: {GetHashCode()}, Thread: {Thread.CurrentThread.ManagedThreadId}");
#endif

            foreach (var actionNumber in ActionNumbers)
            {
                CurrentActionNumber = actionNumber;
                if (Failed)
                    QAcidDebug.WriteLine($"[FAILED SHRINK] Key={key}, Value={value}, Exception={Exception?.Message}");
                Runner(this);
            }
            var failed = Failed;
            CurrentActionNumber = runNumber;
            Failed = false;
            FailingSpec = failingSpec;
            Exception = exception;
            Verifying = false;
            Shrinking = true;
            Memory.ForThisAction().Set(key, oldVal);

            return failed;
        }

        private void Report()
        {
            Report(new QAcidReport());
        }

        private QAcidReport AddMemoryToReport(QAcidReport report)
        {
            report.AddEntry(new QAcidReportShrinkingInfoEntry("shrinking") { NrOfActions = ActionNumbers.Count, ShrinkCount = shrinkCount });
            foreach (var actionNumber in ActionNumbers.ToList())
            {
                Memory.AddActionToReport(actionNumber, report, Exception);
            }
            if (!string.IsNullOrEmpty(FailingSpec))
                report.AddEntry(new QAcidReportSpecEntry(FailingSpec));
            return report;
        }

        private QAcidReport Report(QAcidReport report)
        {
            AddMemoryToReport(report);
            //QAcidDebug.WriteLine(Memory.ToDiagnosticString());
            if (DontThrowFalsifiableException)
                return report;
            throw new FalsifiableException(report.ToString(), Exception)
            {
                QAcidReport = report,
                MemoryDump = Memory.ToDiagnosticString()
            };
        }
    }
}
