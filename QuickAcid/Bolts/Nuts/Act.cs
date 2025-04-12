using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
		=> TryCatch(key, func);

	public static QAcidRunner<TOutput> ActIf<TOutput>(this string key, Func<bool> predicate, Func<TOutput> func)
		=> state => predicate() ? key.Act(func)(state) : QAcidResult.None<TOutput>(state);

	public static QAcidRunner<Acid> Act(this string key, Action action)
		=> TryCatch(key, () => { action(); return Acid.Test; });

	public static QAcidRunner<Acid> ActIf(this string key, Func<bool> predicate, Action func)
		=> state => predicate() ? key.Act(func)(state) : QAcidResult.AcidOnly(state);

	private static QAcidRunner<T> TryCatch<T>(string key, Func<T> func)
	{
		return state =>
		{
			state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.ActionRunner });
			state.GetExecutionContext().memory.ActionKey = key;
			try
			{
				return QAcidResult.Some(state, func());
			}
			catch (Exception ex)
			{
				state.RecordFailure(ex);
				return QAcidResult.None<T>(state);
			}
		};
	}
}