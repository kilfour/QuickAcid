using QuickAcid.Bolts;
using QuickAcid.Bolts.ShrinkStrats;
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
		var execution = state.GetExecutionContext();
		switch (state.CurrentPhase)
		{
			case QAcidPhase.ShrinkInputEval:
			case QAcidPhase.ShrinkingExecutions:
			case QAcidPhase.ShrinkingActions:
				return execution.memory.ContainsKey(key) ?
					QAcidResult.Some(state, execution.memory.Get<T>(key))
					: QAcidResult.None<T>(state);

			case QAcidPhase.ShrinkingInputs
				when execution.AlreadyTried(key):
				{
					var value = generator(state.FuzzState).Value;
					execution.SetIfNotAlreadyThere(key, value);
					return QAcidResult.Some(state, value);
				}

			case QAcidPhase.ShrinkingInputs
				when !execution.AlreadyTried(key):
				{
					var value = execution.Get<T>(key);
					ShrinkStrategyPicker.Input(state, key, value, key);
					execution.SetShrinkOutcome(key);
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
