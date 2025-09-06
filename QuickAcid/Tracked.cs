using QuickAcid.Bolts;
using QuickPulse.Bolts;

namespace QuickAcid;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Tracked<T>(this string key, Func<T> func) =>
		state => Vessel.Some(state, state.Memory.StoreTracked(key, func));

	public static QAcidScript<Box<T>> Tracked<T>(this string key, T initial)
		=> key.Tracked(() => new Box<T>(initial));
}
