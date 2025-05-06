namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{

	public static QAcidRunner<T> Stashed<T>(this string key, Func<T> func)
	{
		return
			state =>
				{
					return QAcidResult.Some(state, state.Memory.StoreStashed(key, func));
				};
	}

	public static QAcidRunner<T> StashedValue<T>(this string key, T initial)
	{
		return key.Stashed(() => new Box<T>(initial)).Select(box => box.Value);
	}

	private class Box<T> { public T Value; public Box(T value) => Value = value; }
}
