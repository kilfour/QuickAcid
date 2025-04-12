using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts;


public static partial class QAcid
{
	public static QAcidRunner<T> ShrinkableInput<T>(this string key, Generator<T> generator)
	{
		return ShrinkableInput(key, generator, _ => true);
	}

	public static QAcidRunner<T> ShrinkableInput<T>(this string key, Generator<T> generator, Func<T, bool> shrinkingGuard)
	{
		return state =>
			{
				var executionContext = state.GetExecutionContext();

				if (state.CurrentPhase == QAcidPhase.ShrinkInputEval)
				{
					return QAcidResult.Some(state, state.Memory.ForThisExecution().Get<T>(key));
				}

				if (state.CurrentPhase == QAcidPhase.ShrinkingExecutions)
				{
					return QAcidResult.Some(state, state.Memory.ForThisExecution().Get<T>(key));
				}

				if (state.CurrentPhase == QAcidPhase.ShrinkingInputs && !executionContext.AlreadyTried(key))
				{
					var value = executionContext.Get<T>(key);
					var shrunk = Shrink.Input(state, key, value, obj => shrinkingGuard((T)obj));
					executionContext.SetShrinkOutcome(key, shrunk);
					return QAcidResult.Some(state, value);
				}

				var value2 = generator.Generate();
				state.Memory.ForThisExecution().SetIfNotAllReadyThere(key, value2);
				return QAcidResult.Some(state, value2);
			};
	}
}
