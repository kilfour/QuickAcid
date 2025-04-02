namespace QuickAcid
{
    public class QAcidState
    {
        public QAcidRunner<Acid> Runner { get; private set; }

        public int RunNumber { get; private set; }
        public List<int> Runs { get; private set; }

        public Memory Memory { get; private set; }
        public SimpleMemory GlobalMemory { get; private set; }
        public SimpleMemory TempMemory { get; private set; }

        public bool Shrinking { get; private set; }
        public Memory Shrunk { get; private set; }

        public bool Verifying { get; private set; }

        public bool Reporting { get; private set; }

        public string FailingSpec { get; private set; }
        public bool Failed { get; private set; }
        public Exception Exception { get; private set; }

        private QAcidReport report = new QAcidReport();

        public QAcidState(QAcidRunner<Acid> runner)
        {
            Runner = runner;
            Runs = new List<int>();
            Memory = new Memory(this);
            GlobalMemory = new SimpleMemory();
            TempMemory = new SimpleMemory();
            Shrunk = new Memory(this);
        }

        public void Run(int times)
        {
            for (int j = 0; j < times; j++)
            {
                Run();
                if (Failed)
                    return;
            }
        }

        public void Run()
        {
            Runs.Add(RunNumber);
            Runner(this);
            if (Failed)
            {
                HandleFailure();
                return;
            }
            RunNumber++;
        }

        public bool ShrinkRun(object key, object value)
        {
            Verifying = true;
            Shrinking = false;
            Reporting = false;
            var tempMemory = TempMemory;
            Failed = false;
            TempMemory = new SimpleMemory();

            var failingSpec = FailingSpec;
            var exception = Exception;
            var runNumber = RunNumber;
            var oldVal = Memory.Get<object>(key);
            Memory.Set(key, value);
            foreach (var run in Runs)
            {
                RunNumber = run;
                Runner(this);
            }

            var failed = Failed;
            RunNumber = runNumber;
            Failed = false;
            FailingSpec = failingSpec;
            Exception = exception;

            Verifying = false;
            Shrinking = true;

            Memory.Set(key, oldVal);
            TempMemory = tempMemory;

            return failed;
        }

        public bool IsNormalRun()
        {
            return (Verifying == false && Shrinking == false && Reporting == false);
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
            Reporting = false;
            BreakRun = false;

            Failed = false;
            var failingSpec = FailingSpec;
            var exception = Exception;

            var max = Runs.Max();
            var current = 0;

            while (current <= max)
            {
                Failed = false;
                TempMemory = new SimpleMemory();
                FailingSpec = failingSpec;
                Exception = exception;

                foreach (var run in Runs.ToList())
                {
                    RunNumber = run;
                    if (run != current)
                        Runner(this);
                    if (BreakRun)
                        break;
                }
                if (Failed && !BreakRun)
                {
                    Runs.Remove(current);
                }
                current++;
            }

            Failed = true;
            FailingSpec = failingSpec;
            Exception = exception;
        }

        private void ShrinkInputs()
        {
            Verifying = false;
            Shrinking = true;
            Reporting = false;

            Failed = false;
            TempMemory = new SimpleMemory();

            var failingSpec = FailingSpec;
            var exception = Exception;

            foreach (var run in Runs.ToList())
            {
                RunNumber = run;
                Runner(this);
            }

            Failed = true;
            FailingSpec = failingSpec;
            Exception = exception;
        }

        private void Report()
        {
            report = new QAcidReport();
            TempMemory = new SimpleMemory();
            Verifying = false;
            Shrinking = false;
            Reporting = true;

            Failed = false;
            var failingSpec = FailingSpec;
            var exception = Exception;

            foreach (var run in Runs.ToList())
            {
                RunNumber = run;
                Runner(this);
            }

            Failed = true;
            FailingSpec = failingSpec;
            Exception = exception;

            if (Exception != null)
            {
                throw new FalsifiableException(report.ToString(), exception) { QAcidReport = report };
            }

            throw new FalsifiableException(report.ToString()) { QAcidReport = report };
        }
    }
}