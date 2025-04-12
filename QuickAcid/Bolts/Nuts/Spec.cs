using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<Acid> Spec(this string key, Func<bool> condition)
	{
		return
			state =>
			{
				state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.SpecRunner });
				if (ShouldSkipSpec(key, state))
					return QAcidResult.AcidOnly(state);
				bool passed = condition();
				if (!passed)
					state.CurrentContext.SpecFailed(key);
				return QAcidResult.AcidOnly(state);
			};
	}
	public static QAcidRunner<Acid> SpecIf(this string key, Func<bool> predicate, Func<bool> condition)
		=> state => predicate() ? key.Spec(condition)(state) : QAcidResult.AcidOnly(state);

	private static bool ShouldSkipSpec(string key, QAcidState state)
		=> state.OriginalRun.FailingSpec is { } failedKey && failedKey != key;
}