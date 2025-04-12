using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
	{
		return
			state =>
				{
					state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.ActionRunner });
					state.Memory.ForThisExecution().ActionKey = key;
					try
					{
						var result = QAcidResult.Some(state, func());
						return result;
					}
					catch (Exception exception)
					{
						state.Memory.ForThisExecution().LastException = exception;
						state.FailedWithException(exception);
						return QAcidResult.None<TOutput>(state);
					}
				};
	}

	public static QAcidRunner<TOutput> ActIf<TOutput>(this string key, Func<bool> predicate, Func<TOutput> func)
	{
		return
			state =>
			{
				if (!predicate())
					return QAcidResult.None<TOutput>(state);
				return key.Act(func)(state);
			};
	}

	public static QAcidRunner<Acid> Act(this string key, Action action)
	{
		return
			state =>
			{
				state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.ActionRunner });
				state.Memory.ForThisExecution().ActionKey = key;
				try
				{
					action();
					return QAcidResult.None<Acid>(state);
				}
				catch (Exception exception)
				{
					state.Memory.ForThisExecution().LastException = exception;
					state.FailedWithException(exception);
					return QAcidResult.AcidOnly(state);
				}
			};
	}

	public static QAcidRunner<Acid> ActIf(this string key, Func<bool> predicate, Action func)
	{
		return
			state =>
			{
				if (!predicate())
					return QAcidResult.AcidOnly(state);
				return key.Act(func)(state);
			};
	}
}