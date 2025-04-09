using QuickAcid.CodeGen;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

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

	public static void Verify(this QAcidRunner<Acid> runner)
	{
		runner.Verify(1, 1);
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
		return runner.ReportIfFailed(1, 1);
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

