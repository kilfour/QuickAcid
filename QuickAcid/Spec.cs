using QuickAcid.Bolts;

namespace QuickAcid;

public static partial class QAcidCombinators
{
	private static QAcidScript<Acid> InnerSpec(this string key, Func<bool> condition) =>
		state =>
			{
				if (ShouldSkipSpec(key, state))
					return QAcidResult.AcidOnly(state);
				if (condition())
					state.SpecPassed(key);
				else
					state.Shifter.CurrentContext.MarkFailure(key);
				return QAcidResult.AcidOnly(state);
			};


	public static QAcidScript<Acid> Spec(this string key, Func<bool> condition)
		=> InnerSpec(key, condition);

	public static QAcidScript<Acid> SpecIf(this string key, Func<bool> predicate, Func<bool> condition)
		=> state => predicate() ? key.InnerSpec(condition)(state) : QAcidResult.AcidOnly(state);

	private static bool ShouldSkipSpec(string key, QAcidState state)
		=> state.Shifter.OriginalRun.FailingSpec is { } failedKey && failedKey != key;

	public static QAcidScript<Acid> Analyze(this string key, Func<bool> condition)
		=> state => state.IsThisTheRunsLastExecution() ? key.InnerSpec(condition)(state) : QAcidResult.AcidOnly(state);

	public static QAcidScript<Acid> Assay(this string key, Func<bool> condition)
		=> state =>
			{
				if (!state.IsThisTheRunsLastExecution())
					return QAcidResult.AcidOnly(state);
				bool passed = condition();
				if (!passed)
				{
					state.AllowShrinking = false;
					state.Shifter.CurrentContext.MarkFailure(key);
				}
				else
				{
					state.SpecPassed(key);
				}
				return QAcidResult.AcidOnly(state);
			};

	public static QAcidScript<Acid> Assay(this string key, params (string label, Func<bool> condition)[] specs) =>
		state =>
			{
				if (!state.IsThisTheRunsLastExecution())
					return QAcidResult.AcidOnly(state);

				bool passed = true;
				var strings = new List<string>();
				foreach (var (label, condition) in specs)
				{
					if (!condition())
					{
						strings.Add(label);
						passed = false;
					}
				}
				if (!passed)
				{
					state.AllowShrinking = false;
					state.Shifter.CurrentContext.MarkFailure(string.Join(", ", strings));
				}
				else
				{
					state.SpecPassed(key);
				}
				return QAcidResult.AcidOnly(state);
			};

	public static QAcidScript<Acid> TestifyProvenWhen(this string key, Func<bool> condition) =>
		state =>
			{
				if (ShouldSkipSpec(key, state))
					return QAcidResult.AcidOnly(state);
				bool passed = condition();
				if (passed)
				{
					state.Shifter.CurrentContext.StopRun();
				}
				return QAcidResult.AcidOnly(state);
			};
}