namespace QuickAcid.Bolts;

public static class QAcidRunnerToLinq
{
	public static QAcidRunner<TValueTwo> Select<TValueOne, TValueTwo>(
		this QAcidRunner<TValueOne> runner,
		Func<TValueOne, TValueTwo> selector)
	{
		return
			state =>
			{
				if (state.Failed)
					return QAcidResult.None<TValueTwo>(state);
				return QAcidResult.Some(state, selector(runner(state).Value));
			};
	}

	// This is the Bind function
	public static QAcidRunner<TResult> SelectMany<TSource, TResult>(
		this QAcidRunner<TSource> source,
		Func<TSource, QAcidRunner<TResult>> selector)
	{
		return state =>
		{
			if (state.Failed)
				return QAcidResult.None<TResult>(state);
			var result = source(state);
			if (state.Failed)
				return QAcidResult.None<TResult>(state);
			return selector(result.Value)(state);
		};
	}

	public static QAcidRunner<TValueThree> SelectMany<TValueOne, TValueTwo, TValueThree>(
		this QAcidRunner<TValueOne> runner,
		Func<TValueOne, QAcidRunner<TValueTwo>> selector,
		Func<TValueOne, TValueTwo, TValueThree> projector)
	{
		return runner.SelectMany(x => selector(x).Select(y => projector(x, y)));
	}
}
