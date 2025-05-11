namespace QuickAcid.Bolts.Nuts;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Tracked<T>(this string key, Func<T> func)
	{
		return Tracked(key, func, QuickAcidStringify.Default<T>());
	}

	public static QAcidScript<T> Tracked<T>(this string key, Func<T> func, Func<T, string> stringify)
	{
		return
			state =>
				{
					return QAcidResult.Some(state, state.Memory.StoreTracked(key, func, stringify));
				};
	}

	public static QAcidScript<Box<T>> Tracked<T>(this string key, T initial)
	{
		return key.Tracked(() => new Box<T>(initial));
	}
}
