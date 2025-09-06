using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;

namespace QuickAcid;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Capture<T>(this string key, Func<T> func) =>
		state => Vessel.Some(state, state.CurrentExecutionContext().Remember(key, func, ReportingIntent.Never));

	public static QAcidScript<T> CaptureIf<T>(this string key, Func<bool> predicate, Func<T> func) =>
		state => predicate() ? Capture(key, func)(state) : Vessel.None<T>(state);
}
