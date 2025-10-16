using System.Runtime.CompilerServices;
using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using StringExtensionCombinators;

namespace QuickAcid;

public static partial class Script
{
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

	public static QAcidScript<T> ChooseIf<T>(params (Func<bool> condition, QAcidScript<T> script)[] scripts)
		=> ChooseIfInternal(scripts);

	private static QAcidScript<T> ChooseIfInternal<T>(
		 (Func<bool> condition, QAcidScript<T> script)[] scripts,
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0,
		[CallerMemberName] string member = "")
	{
		var key = $"Choose|{Path.GetFileName(file)}:{line}|{member}|{scripts.Length}";
		return state =>
			{
				var validScriptIndexes =
					scripts.Select((a, ix) => (ix, a.condition))
						.Where(a => a.condition())
						.Select(a => a.ix);
				var index = state.CurrentExecutionContext().Remember(key,
					() => Fuzz.ChooseFrom(validScriptIndexes)(state.FuzzState).Value, ReportingIntent.Never);
				var script = scripts[index];
				if (script.condition())
					return scripts[index].script(state);
				return Vessel.None<T>(state);
			};
	}
}
