using System.Runtime.CompilerServices;
using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid;

public static class Script
{
	public static QAcidScript<Acid> Execute(Action action) =>
		state => { action(); return QAcidResult.AcidOnly(state); };

	public static QAcidScript<Acid> ExecuteIf(Func<bool> predicate, Action action) =>
		state => { if (predicate()) action(); return QAcidResult.AcidOnly(state); };
	public static QAcidScript<T> Execute<T>(Func<T> func) =>
		state => QAcidResult.Some(state, func());

	public static QAcidScript<T> ExecuteIf<T>(Func<bool> predicate, Func<T> func) =>
		state => predicate() ? QAcidResult.Some(state, func()) : QAcidResult.None<T>(state);

	public static QAcidScript<T> Execute<T>(Generator<T> generator) =>
		state => QAcidResult.Some(state, generator(state.FuzzState).Value);

	public static QAcidScript<T> ExecuteIf<T>(Func<bool> predicate, Generator<T> generator) =>
		state => predicate() ? QAcidResult.Some(state, generator(state.FuzzState).Value) : QAcidResult.None<T>(state);

	public static QAcidScript<T> Choose<T>(params QAcidScript<T>[] scripts)
		=> ChooseInternal(scripts);

	private static QAcidScript<T> ChooseInternal<T>(
		QAcidScript<T>[] scripts,
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0,
		[CallerMemberName] string member = "")
	{
		var key = $"Choose|{Path.GetFileName(file)}:{line}|{member}|{scripts.Length}";
		return state =>
			{
				var index = state.CurrentExecutionContext().Remember(key,
					() => Fuzz.Int(0, scripts.Length)(state.FuzzState).Value, ReportingIntent.Never);
				return scripts[index](state);
			};
	}
}
