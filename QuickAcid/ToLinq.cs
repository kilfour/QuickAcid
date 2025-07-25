﻿using QuickAcid.Bolts;

namespace QuickAcid;

public static class ToLinq
{
	public static QAcidScript<TValueTwo> Select<TValueOne, TValueTwo>(
		this QAcidScript<TValueOne> script,
		Func<TValueOne, TValueTwo> selector) =>
			state =>
				{
					if (state.Shifter.CurrentContext.NeedsToStop())
						return QAcidResult.None<TValueTwo>(state);
					return QAcidResult.Some(state, selector(script(state).Value));
				};

	public static QAcidScript<TResult> SelectMany<TSource, TResult>(
		this QAcidScript<TSource> source,
		Func<TSource, QAcidScript<TResult>> selector) =>
			state =>
				{
					if (state.Shifter.CurrentContext.NeedsToStop())
						return QAcidResult.None<TResult>(state);
					var result = source(state);
					if (state.Shifter.CurrentContext.NeedsToStop())
						return QAcidResult.None<TResult>(state);
					return selector(result.Value)(state);
				};

	public static QAcidScript<TValueThree> SelectMany<TValueOne, TValueTwo, TValueThree>(
		this QAcidScript<TValueOne> script,
		Func<TValueOne, QAcidScript<TValueTwo>> selector,
		Func<TValueOne, TValueTwo, TValueThree> projector)
			=> script.SelectMany(x => selector(x).Select(y => projector(x, y)));
}
