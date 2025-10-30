using QuickAcid.Bolts;
using QuickAcid;
using QuickPulse.Bolts;

namespace StringExtensionCombinators;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Tracked<T>(this string key, Func<T> func) =>
		state => Vessel.Some(state, state.Memory.StoreTracked(key, func));

	public static QAcidScript<Cell<T>> Tracked<T>(this string key, T initial)
		=> key.Tracked(() => new Cell<T>(initial));
}
