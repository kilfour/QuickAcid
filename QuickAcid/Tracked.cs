using QuickAcid.Bolts;
using QuickPulse.Bolts;

namespace QuickAcid;

public static partial class QAcidCombinators
{

	public static QAcidScript<T> Tracked<T>(this string key, Func<T> func)
	{
		return
			state =>
				{
					return QAcidResult.Some(state, state.Memory.StoreTracked(key, func));
				};
	}

	public static QAcidScript<Box<T>> Tracked<T>(this string key, T initial)
	{
		return key.Tracked(() => new Box<T>(initial));
	}
}
