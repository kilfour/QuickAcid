using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> AlwaysReported<T>(this string key, Func<T> func)
	{
		return AlwaysReported(key, func, QuickAcidStringify.Default<T>());
	}

	public static QAcidRunner<T> AlwaysReported<T>(this string key, Func<T> func, Func<T, string> stringify)
	{

		return
			state =>
				{
					state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.AlwaysReportedInputRunner });

					var value = state.Memory.StoreAlwaysReported(key, func, stringify);

					// var value = state.Memory.AlwaysReportedInputsPerRun.GetOrAdd(key, func, stringify);
					// state.Memory.AddAlwaysReportedInputValueForCurrentRun(key, stringify(value));

					return QAcidResult.Some(state, value);
				};
	}
}
