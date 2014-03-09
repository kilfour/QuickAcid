using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public class QAcidState
	{
		public QAcidRunner<Unit> Runner { get; set; }

		public int RunNumber { get; set; }
		public List<int> Runs { get; set; }

		public Memory Memory { get; set; }
		public GlobalMemory TempMemory { get; set; }

		public string FailingSpec { get; set; }
		public bool Failed { get; set; }

		public bool Shrinking { get; set; }
		public Memory Shrunk { get; set; }

		public bool Verifying { get; set; }

		public bool Reporting { get; set; }
		public Exception Exception { get; set; }

		public QAcidState()
		{
			Runs = new List<int>();
			Memory = new Memory(this);
			TempMemory = new GlobalMemory();
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

		public void HandleFailure()
		{
			ShrinkActions();
			ShrinkInputs();
			Report();
		}

		public void ShrinkActions()
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
				TempMemory = new GlobalMemory();
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

		public void ShrinkInputs()
		{
			Verifying = false;
			Shrinking = true;
			Reporting = false;

			Failed = false;
			TempMemory = new GlobalMemory();

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

		public bool ShrinkRun(object key, object value)
		{
			Verifying = true;
			Shrinking = false;
			Reporting = false;
			var tempMemory = TempMemory;
			Failed = false;
			TempMemory = new GlobalMemory();

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

		private StringBuilder reportBuilder;

		public void Report()
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

		public void LogReport(string report)
		{
			reportBuilder.AppendLine(report);
		}

		public bool IsNormalRun()
		{
			return (Verifying == false && Shrinking == false && Reporting == false);
		}
	}

	public class FalsifiableException : Exception
	{
		public FalsifiableException(string message)
			: base(message) { }

		public FalsifiableException(string message, Exception exception)
			: base(message, exception) { }
	}
}