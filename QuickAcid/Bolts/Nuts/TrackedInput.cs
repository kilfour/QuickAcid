using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> AlwaysReported<T>(this string key, Func<T> func)
	{
		return AlwaysReported(key, func, t => t == null ? "null" : t.ToString());
	}

	public static QAcidRunner<T> AlwaysReported<T>(this string key, Func<T> func, Func<T, string> stringify)
	{
		return
			s =>
				{
					s.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.AlwaysReportedInputRunner });
					var value = s.Memory.AlwaysReportedInputsPerRun.GetOrAdd(key, func, stringify);
					//if (!s.Shrinking)
					s.Memory.AddAlwaysReportedInputValueForCurrentRun(key, stringify(value));
					return QAcidResult.Some(s, value);
				};
	}
}
