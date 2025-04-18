﻿using QuickAcid.CodeGen;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public static partial class QAcid
{
	public static void Verify(this QAcidRunner<Acid> runner, int scopes, int executionsPerScope)
	{
		for (int i = 0; i < scopes; i++)
		{
			var state = new QAcidState(runner);
			state.Run(executionsPerScope);
			state.ThrowFalsifiableExceptionIfFailed();
		}
	}

	public static void Verify(this QAcidRunner<Acid> runner)
	{
		runner.Verify(1, 1);
	}

	public static QAcidReport ReportIfFailed(this QAcidRunner<Acid> runner, int scopes, int executionsPerScope)
	{
		for (int i = 0; i < scopes; i++)
		{
			var state = new QAcidState(runner);
			state.Run(executionsPerScope);
			if (state.CurrentContext.Failed)
				return state.GetReport();
		}
		return null!;
	}

	public static QAcidReport ReportIfFailed(this QAcidRunner<Acid> runner)
	{
		return runner.ReportIfFailed(1, 1);
	}

	public static string ToCodeIfFailed(this QAcidRunner<Acid> runner, int scopes, int executionsPerScope)
	{
		for (int i = 0; i < scopes; i++)
		{
			var state = new QAcidState(runner);
			state.Run(executionsPerScope);
			if (state.CurrentContext.Failed)
				return Prospector.Pan(state);
		}
		return null!;
	}
}

