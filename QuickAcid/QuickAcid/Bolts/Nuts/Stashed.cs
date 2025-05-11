namespace QuickAcid.Bolts.Nuts;

public static partial class QAcidCombinators
{

	public static QAcidScript<T> Stashed<T>(this string key, Func<T> func)
	{
		return
			state =>
				{
					return QAcidResult.Some(state, state.Memory.StoreStashed(key, func));
				};
	}

	public static QAcidScript<Box<T>> StashedValue<T>(this string key, T initial)
	{
		return key.Stashed(() => new Box<T>(initial));
	}
}
