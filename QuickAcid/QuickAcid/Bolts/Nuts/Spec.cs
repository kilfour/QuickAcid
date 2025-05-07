using QuickAcid.CodeGen;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcidCombinators
{
	private static QAcidRunner<Acid> InnerSpec(this string key, Func<bool> condition) =>
		state =>
			{
				if (ShouldSkipSpec(key, state))
					return QAcidResult.AcidOnly(state);
				bool passed = condition();
				if (!passed)
				{
					state.CurrentContext.MarkFailure(key);
				}
				return QAcidResult.AcidOnly(state);
			};


	public static QAcidRunner<Acid> Spec(this string key, Func<bool> condition)
		=> InnerSpec(key, condition);

	public static QAcidRunner<Acid> SpecIf(this string key, Func<bool> predicate, Func<bool> condition)
		=> state => predicate() ? key.InnerSpec(condition)(state) : QAcidResult.AcidOnly(state);

	private static bool ShouldSkipSpec(string key, QAcidState state)
		=> state.OriginalRun.FailingSpec is { } failedKey && failedKey != key;

	public static QAcidRunner<Acid> Analyze(this string key, Func<bool> condition)
		=> state => state.IsThisTheRunsLastExecution() ? key.InnerSpec(condition)(state) : QAcidResult.AcidOnly(state);

	public static QAcidRunner<Acid> Assay(this string key, Func<bool> condition)
		=> state =>
			{
				if (!state.IsThisTheRunsLastExecution())
					return QAcidResult.AcidOnly(state);
				bool passed = condition();
				if (!passed)
				{
					state.AllowShrinking = false;
					state.CurrentContext.MarkFailure(key);
				}
				return QAcidResult.AcidOnly(state);
			};

	public static QAcidRunner<Acid> Assay(this string key, params (string label, Func<bool> condition)[] specs) =>
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
					state.CurrentContext.MarkFailure(string.Join(", ", strings));
				}
				return QAcidResult.AcidOnly(state);
			};

	public static QAcidRunner<Acid> TestifyProvenWhen(this string key, Func<bool> condition) =>
		state =>
			{
				if (ShouldSkipSpec(key, state))
					return QAcidResult.AcidOnly(state);
				bool passed = condition();
				if (passed)
				{
					state.CurrentContext.BreakRun = true;
				}
				return QAcidResult.AcidOnly(state);
			};
}