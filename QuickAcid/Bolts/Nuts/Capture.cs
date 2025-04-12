namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Capture<T>(this string key, Func<T> func, Func<T, string> stringify = null)
	{
		return state =>
		{
			return QAcidResult.Some(state, state.Remember(key, func));
		};
	}
}
