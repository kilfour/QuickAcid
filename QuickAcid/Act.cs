using QuickAcid.Bolts;
using QuickAcid.Phasers;

namespace QuickAcid;

public static partial class QAcidCombinators
{
	private static QAcidScript<TOutput> TryCatch<TOutput>(string key, Func<TOutput> func)
	=> state =>
		{
			var execution = state.CurrentExecutionContext();
			if (state.Shifter.CurrentPhase == QAcidPhase.NormalRun)
				execution.access.ActionKeys.Add(key);
			var needsToAct = execution.access.ActionKeys.Contains(key);
			if (needsToAct)
			{
				try
				{
					return QAcidResult.Some(state, func());
				}
				catch (Exception ex)
				{
					state.RecordFailure(ex);
					return QAcidResult.None<TOutput>(state);
				}
			}
			return QAcidResult.None<TOutput>(state);
		};

	private static QAcidScript<TOut> TryCapture<TOut>(this string key, Func<TOut> func, Func<Exception, TOut> onError)
		=> state =>
	{
		state.CurrentExecutionContext().access.ActionKeys.Add(key);
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

	public static QAcidScript<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
		=> TryCatch(key, func);

	public static QAcidScript<Acid> Act(this string key, Action action)
		=> TryCatch(key, () => { action(); return Acid.Test; });

	public static QAcidScript<TOutput> ActIf<TOutput>(this string key, Func<bool> predicate, Func<TOutput> func)
		=> state => predicate() ? key.Act(func)(state) : QAcidResult.None<TOutput>(state);

	public static QAcidScript<Acid> ActIf(this string key, Func<bool> predicate, Action func)
		=> state => predicate() ? key.Act(func)(state) : QAcidResult.AcidOnly(state);
	// public static QAcidScript<Acid> ActOnce(this string key, Func<bool> predicate, Action func)
	// 	=> state =>
	// 		{
	// 			var flag = state.Memory.StoreStashed(key, () => false);
	// 			if (flag)
	// 			{

	// 			}
	// 			return predicate() ? key.Act(func)(state) : QAcidResult.AcidOnly(state);
	// 		};
	public static QAcidScript<QAcidDelayedResult> ActCarefully(this string key, Action action)
		=> key.TryCapture(() => { action(); return new QAcidDelayedResult(); }, ex => new QAcidDelayedResult(ex));

	public static QAcidScript<QAcidDelayedResult<T>> ActCarefully<T>(this string key, Func<T> func)
		=> key.TryCapture(() => new QAcidDelayedResult<T>(func()), ex => new QAcidDelayedResult<T>(ex));

}