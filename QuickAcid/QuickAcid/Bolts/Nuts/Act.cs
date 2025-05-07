namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	private static QAcidRunner<TOutput> TryCatch<TOutput>(string key, Func<TOutput> func)
	=> state =>
		{
			state.GetExecutionContext().memory.ActionKeys.Add(key);
			try
			{
				return QAcidResult.Some(state, func());
			}
			catch (Exception ex)
			{
				state.RecordFailure(ex);
				return QAcidResult.None<TOutput>(state);
			}
		};

	private static QAcidRunner<TOut> TryCapture<TOut>(this string key, Func<TOut> func, Func<Exception, TOut> onError)
		=> state =>
	{
		state.GetExecutionContext().memory.ActionKeys.Add(key);
		try
		{
			var result = func();
			return QAcidResult.Some(state, result);
		}
		catch (Exception ex)
		{
			return QAcidResult.Some(state, onError(ex));
		}
	};

	public static QAcidRunner<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
		=> TryCatch(key, func);

	public static QAcidRunner<Acid> Act(this string key, Action action)
		=> TryCatch(key, () => { action(); return Acid.Test; });

	public static QAcidRunner<TOutput> ActIf<TOutput>(this string key, Func<bool> predicate, Func<TOutput> func)
		=> state => predicate() ? key.Act(func)(state) : QAcidResult.None<TOutput>(state);

	public static QAcidRunner<Acid> ActIf(this string key, Func<bool> predicate, Action func)
		=> state => predicate() ? key.Act(func)(state) : QAcidResult.AcidOnly(state);
	// public static QAcidRunner<Acid> ActOnce(this string key, Func<bool> predicate, Action func)
	// 	=> state =>
	// 		{
	// 			var flag = state.Memory.StoreStashed(key, () => false);
	// 			if (flag)
	// 			{

	// 			}
	// 			return predicate() ? key.Act(func)(state) : QAcidResult.AcidOnly(state);
	// 		};
	public static QAcidRunner<QAcidDelayedResult> ActCarefully(this string key, Action action)
		=> key.TryCapture(() => { action(); return new QAcidDelayedResult(); }, ex => new QAcidDelayedResult(ex));

	public static QAcidRunner<QAcidDelayedResult<T>> ActCarefully<T>(this string key, Func<T> func)
		=> key.TryCapture(() => new QAcidDelayedResult<T>(func()), ex => new QAcidDelayedResult<T>(ex));

}