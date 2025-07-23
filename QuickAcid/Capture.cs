using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Bolts;

namespace QuickAcid;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Capture<T>(this string key, Func<T> func) =>
		state => QAcidResult.Some(state, state.CurrentExecutionContext().Remember(key, func, ReportingIntent.Never));
	public static QAcidScript<T> CaptureIf<T>(this string key, Func<bool> predicate, Func<T> func) =>
		state => predicate() ? Capture(key, func)(state) : QAcidResult.None<T>(state);
}
