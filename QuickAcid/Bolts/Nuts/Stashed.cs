using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Stashed<T>(this string key, Func<T> func)
	{
		return Stashed(key, func, QuickAcidStringify.Default<T>());
	}

	public static QAcidRunner<T> Stashed<T>(this string key, Func<T> func, Func<T, string> stringify)
	{
		return
			state =>
				{
					return QAcidResult.Some(state, state.Memory.StoreAlwaysReported(key, func, stringify, false));
				};
	}
}
