namespace QuickAcid.Bolts;

public static class ToLinq
{
	public static QAcidScript<TValueTwo> Select<TValueOne, TValueTwo>(
		this QAcidScript<TValueOne> runner,
		Func<TValueOne, TValueTwo> selector) =>
			state =>
				{
					if (state.CurrentContext.Failed)
						return QAcidResult.None<TValueTwo>(state);
					return QAcidResult.Some(state, selector(runner(state).Value));
				};

	// This is the Bind function
	public static QAcidScript<TResult> SelectMany<TSource, TResult>(
		this QAcidScript<TSource> source,
		Func<TSource, QAcidScript<TResult>> selector) =>
			state =>
				{
					if (state.CurrentContext.Failed)
						return QAcidResult.None<TResult>(state);
					var result = source(state);
					if (state.CurrentContext.Failed)
						return QAcidResult.None<TResult>(state);
					return selector(result.Value)(state);
				};

	public static QAcidScript<TValueThree> SelectMany<TValueOne, TValueTwo, TValueThree>(
		this QAcidScript<TValueOne> runner,
		Func<TValueOne, QAcidScript<TValueTwo>> selector,
		Func<TValueOne, TValueTwo, TValueThree> projector)
			=> runner.SelectMany(x => selector(x).Select(y => projector(x, y)));
}
