using QuickAcid.Bolts;

namespace QuickAcid;

public class DelayedSpecResult
{
	public enum DelayedSpecResultState { Passed, Failed, Skipped }
	private readonly QAcidState state;

	public string Label { get; }
	public bool Passed { get { return resultState == DelayedSpecResultState.Passed; } }
	public bool Failed { get { return resultState == DelayedSpecResultState.Failed; } }
	public bool Skipped { get { return resultState == DelayedSpecResultState.Skipped; } }

	private DelayedSpecResultState resultState { get; init; }

	private DelayedSpecResult(QAcidState state, string label, DelayedSpecResultState resultState)
	{
		this.state = state;
		Label = label;
		this.resultState = resultState;
	}

	public DelayedSpecResult(QAcidState state, string label)
		: this(state, label, DelayedSpecResultState.Skipped) { }

	public DelayedSpecResult(QAcidState state, string label, bool passed)
		: this(state, label, passed ? DelayedSpecResultState.Passed : DelayedSpecResultState.Failed) { }

	public Acid Apply()
	{
		if (Failed)
			state.Shifter.CurrentContext.MarkFailure(Label);
		else
			state.SpecPassed(Label);
		return Acid.Test;
	}
}


public static partial class QAcidCombinators
{
	private static QAcidScript<DelayedSpecResult> InnerDelayedSpec(this string key, Func<bool> condition) =>
		state =>
			{
				if (ShouldSkipSpec(key, state))
					return Vessel.Some(state, new DelayedSpecResult(state, key));
				return Vessel.Some(state, new DelayedSpecResult(state, key, condition()));
			};

	public static QAcidScript<DelayedSpecResult> DelayedSpec(this string key, Func<bool> condition)
		=> InnerDelayedSpec(key, condition);

	public static QAcidScript<DelayedSpecResult> DelayedSpecIf(this string key, Func<bool> predicate, Func<bool> condition)
		=> state => predicate()
		? key.InnerDelayedSpec(condition)(state)
		: Vessel.Some(state, new DelayedSpecResult(state, key));
}