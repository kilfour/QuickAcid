using QuickAcid.Bolts;
using QuickAcid.Phasers;
using QuickAcid.Shrinking;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid;


public static partial class QAcidCombinators
{
	public static QAcidScript<T> Input<T>(this string key, Generator<T> generator)
	{
		return state =>
			{
				return state.HandleInput(key, generator);
			};
	}

	private static QAcidResult<T> HandleInput<T>(this QAcidState state, string key, Generator<T> generator)
	{
		var execution = state.CurrentExecutionContext();
		switch (state.Shifter.CurrentPhase)
		{
			case QAcidPhase.ShrinkInputEval:
			case QAcidPhase.ShrinkingExecutions:
			case QAcidPhase.ShrinkingActions:
				return execution.access.ContainsKey(key) ?
					QAcidResult.Some(state, execution.access.Get<T>(key))
					: QAcidResult.None<T>(state);

			case QAcidPhase.ShrinkingInputs
				when execution.AlreadyTriedToShrink(key):
				{
					var value = generator(state.FuzzState).Value;
					execution.SetIfNotAlreadyThere(key, value);
					return QAcidResult.Some(state, value);
				}

			case QAcidPhase.ShrinkingInputs
				when !execution.AlreadyTriedToShrink(key):
				{
					var value = execution.Get<T>(key);
					ShrinkStrategyPicker.Input(state, key, value, key);
					execution.MarkAsTriedToShrink(key);
					return QAcidResult.Some(state, value);
				}

			default:
				{
					var value = generator(state.FuzzState).Value;
					execution.SetIfNotAlreadyThere(key, value);
					return QAcidResult.Some(state, value);
				}
		}
	}
}
