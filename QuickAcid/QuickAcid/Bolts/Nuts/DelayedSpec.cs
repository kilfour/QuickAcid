namespace QuickAcid.Bolts.Nuts;

public class DelayedSpecResult
{
	public enum DelayedSpecResultState { Passed, Failed, Skipped }
	private readonly QAcidState state;

	public string Label { get; }
	public bool Passed { get { return resultState == DelayedSpecResultState.Passed; } }
	public bool Failed { get { return resultState == DelayedSpecResultState.Failed; } }
	public bool Skipped { get { return resultState == DelayedSpecResultState.Skipped; } }
	private DelayedSpecResultState resultState { get; init; }


	public DelayedSpecResult(QAcidState state, string label)
	{
		this.state = state;
		Label = label;
		resultState = DelayedSpecResultState.Skipped;
	}

	public DelayedSpecResult(QAcidState state, string label, bool passed)
	{
		this.state = state;
		Label = label;
		if (passed)
			resultState = DelayedSpecResultState.Passed;
		else
			resultState = DelayedSpecResultState.Failed;
	}

	public Acid Apply()
	{
		if (Failed)
			state.CurrentContext.MarkFailure(Label);
		return Acid.Test;
	}
}


public static partial class QAcidCombinators
{
	private static QAcidScript<DelayedSpecResult> InnerDelayedSpec(this string key, Func<bool> condition) =>
		state =>
			{
				if (ShouldSkipSpec(key, state))
					return QAcidResult.None<DelayedSpecResult>(state);
				return QAcidResult.Some(state, new DelayedSpecResult(state, key, condition()));
			};

	public static QAcidScript<DelayedSpecResult> DelayedSpec(this string key, Func<bool> condition)
		=> InnerDelayedSpec(key, condition);

	public static QAcidScript<DelayedSpecResult> DelayedSpecIf(this string key, Func<bool> predicate, Func<bool> condition)
		=> state => predicate() ? key.InnerDelayedSpec(condition)(state) : QAcidResult.Some(state, new DelayedSpecResult(state, key));
}