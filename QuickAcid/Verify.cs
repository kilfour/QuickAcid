using QuickAcid.CodeGen;
using QuickAcid.Reporting;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static void Verify(this QAcidRunner<Acid> runner, int loops, int actions)
		{
			for (int i = 0; i < loops; i++)
			{
				var state = new QAcidState(runner);
				state.Run(actions);
				state.ThrowFalsifiableExceptionIfFailed();
			}
		}

		public static void VerifyWith(this QAcidRunner<Acid> runner, NumberOfRuns numberOfRuns, ActionsPerRun actionsPerRun)
		{
			Verify(runner, numberOfRuns.Value, actionsPerRun.Value);
		}

		public static void Verify(this QAcidRunner<Acid> runner)
		{
			Verify(runner, 1, 1);
		}

		// todo : implement some thing like runOptions and fluently plug it in
		public static void VerifyVerbose(this QAcidRunner<Acid> runner, NumberOfRuns numberOfRuns, ActionsPerRun actionsPerRun)
		{
			for (int i = 0; i < numberOfRuns.Value; i++)
			{
				var state = new QAcidState(runner) { Verbose = true };
				state.Run(actionsPerRun.Value);
				state.ThrowFalsifiableExceptionIfFailed();
			}
		}

		public static QAcidReport ReportIfFailed(this QAcidRunner<Acid> runner, int runs, int actions)
		{
			for (int i = 0; i < runs; i++)
			{
				var state = new QAcidState(runner);
				state.Run(actions);
				if (state.Failed)
					return state.GetReport();
			}
			return null!;
		}

		public static QAcidReport ReportIfFailed(this QAcidRunner<Acid> runner)
		{
			return ReportIfFailed(runner, 1, 1);
		}

		public static string ToCodeIfFailed(this QAcidRunner<Acid> runner, int runs, int actions)
		{
			for (int i = 0; i < runs; i++)
			{
				var state = new QAcidState(runner);
				state.Run(actions);
				if (state.Failed)
					return Prospector.Pan(state);
			}
			return null!;
		}


	}
	public static class IntExtensionForVerify
	{
		public static NumberOfRuns Runs(this int number)
		{
			return new NumberOfRuns(number);
		}

		public static ActionsPerRun ActionsPerRun(this int number)
		{
			return new ActionsPerRun(number);
		}
	}
	public class NumberOfRuns
	{
		public int Value { get; }
		public NumberOfRuns(int value) { Value = value; }
	}

	public class ActionsPerRun
	{
		public int Value { get; }
		public ActionsPerRun(int value) { Value = value; }
	}
}
