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

        public bool Verifying { get; private set; }


        public string FailingSpec { get; private set; }
        public bool Failed { get; private set; }
        public Exception Exception { get; private set; }

        private QAcidReport report = new QAcidReport();

        private int shrinkCount = 0;

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

        public void LogReport(QAcidReportEntry reportEntry)
        {
            report.AddEntry(reportEntry);
        }

        private void HandleFailure()
        {
            ShrinkActions();
            ShrinkInputs();
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

        public string ShrinkSummary =>
            $"Falsified after {ActionNumbers.Count} runs, {shrinkCount} shrinks";

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
            foreach (var actionNumber in ActionNumbers)
            {
                CurrentActionNumber = actionNumber;
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
            report = new QAcidReport();
            report.ShrinkAttempts = shrinkCount;
            foreach (var actionNumber in ActionNumbers.ToList())
            {
                Memory.AddActionToReport(actionNumber, report);
            }
            if (!string.IsNullOrEmpty(FailingSpec))
                report.AddEntry(new QAcidReportSpecEntry(FailingSpec));
            throw new FalsifiableException(ShrinkSummary + "\n" + report.ToString(), Exception)
            {
                QAcidReport = report
            };
        }
    }
}