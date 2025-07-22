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
				return execution.GetMaybe<T>(key).Match(
					some: x => QAcidResult.Some(state, x),
					none: () =>
					{
						return QAcidResult.None<T>(state);
					});


			case QAcidPhase.FeedbackShrinking
				when !execution.AlreadyTried(key):
				{
					var decoratedValue = execution.GetDecorated(key);
					// var shrunk = ShrinkStrategyPicker.Input(state, key, decoratedValue.Value, key);
					// if (shrunk != decoratedValue.ShrinkOutcome)
					// {
					// 	var number = state.CurrentExecutionNumber;
					// 	state.ShrinkExecutions();
					// 	state.CurrentExecutionNumber = number;
					// }
					// execution.SetShrinkOutcome(key, shrunk);
					return QAcidResult.Some(state, (T)decoratedValue.Value!);
				}

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
