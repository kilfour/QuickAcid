using QuickAcid.Bolts;
using QuickAcid;
using QuickPulse.Bolts;

namespace StringExtensionCombinators;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Stashed<T>(this string key, Func<T> func) =>
		state => Vessel.Some(state, state.Memory.StoreStashed(key, func));

	public static QAcidScript<Box<T>> StashedValue<T>(this string key, T initial)
		=> key.Stashed(() => new Box<T>(initial));
}
