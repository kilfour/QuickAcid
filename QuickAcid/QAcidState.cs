using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public class QAcidState
	{
		public QAcidRunner<Unit> Runner { get; private set; }

		public int RunNumber { get; private set; }
		public List<int> Runs { get; private set; }

		public Memory Memory { get; private set; }
		public TempMemory TempMemory { get; private set; }

		public bool Shrinking { get; private set; }
		public Memory Shrunk { get; private set; }

		public bool Verifying { get; private set; }

		public bool Reporting { get; private set; }

		public string FailingSpec { get; private set; }
		public bool Failed { get; private set; }
		public Exception Exception { get; private set; }


		private StringBuilder reportBuilder;

		public QAcidState(QAcidRunner<Unit> runner)
		{
			Runner = runner;
			Runs = new List<int>();
			Memory = new Memory(this);
			TempMemory = new TempMemory();
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
			TempMemory = new TempMemory();

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
			Failed = true;
			Exception = exception;
		}

		public void SpecFailed(string failingSpec)
		{
			Failed = true;
			FailingSpec = failingSpec;
		}

		public void LogReport(string report)
		{
			reportBuilder.AppendLine(report);
		}

		private void HandleFailure()
		{
			//ShrinkActions();
			ShrinkInputs();
			Report();
		}

		private void ShrinkActions()
		{
			Verifying = true;
			Shrinking = false;
			Reporting = false;

			Failed = false;
			var failingSpec = FailingSpec;
			var exception = Exception;

			var max = Runs.Max(); 
			var current = 0;

			while (current < max)
			{
				Failed = false;
				TempMemory = new TempMemory();
				FailingSpec = failingSpec;
				Exception = exception;

				foreach (var run in Runs.ToList())
				{
					RunNumber = run;
					if (run != current)
						Runner(this);
				}
				if (Failed)
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
			TempMemory = new TempMemory();

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
			reportBuilder = new StringBuilder();
			reportBuilder.AppendLine();
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

			if(Exception != null)
			{
				throw new FalsifiableException(reportBuilder.ToString(), exception);
			}

			throw new FalsifiableException(reportBuilder.ToString());
		}
	}
}