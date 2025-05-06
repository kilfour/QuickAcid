using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;
using QuickAcid.Bolts.ShrinkStrats;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts;


public static partial class QAcid
{
	public static QAcidRunner<T> Shrinkable<T>(this string key, Generator<T> generator)
	{
		if (generator is IKnowMyGuard<T> guarded)
			return key.Shrinkable(generator, guarded.Guard);
		return Shrinkable(key, generator, _ => true);
	}

	public static QAcidRunner<T> Shrinkable<T>(this string key, Generator<T> generator, Func<T, bool> guard)
	{
		return state =>
			{
				return state.HandleShrinkableInput(key, generator, guard);
			};
	}

	private static QAcidResult<T> HandleShrinkableInput<T>(this QAcidState state, string key, Generator<T> generator, Func<T, bool> guard)
	{
		var execution = state.GetExecutionContext();
		switch (state.CurrentPhase)
		{
			case QAcidPhase.ShrinkInputEval:
			case QAcidPhase.ShrinkingExecutions:
				return execution.GetMaybe<T>(key).Match(
					some: x => QAcidResult.Some(state, x),
					none: () => QAcidResult.None<T>(state));

			case QAcidPhase.FeedbackShrinking or QAcidPhase.ShrinkingInputs
				when !execution.AlreadyTried(key):
				{
					var value = execution.Get<T>(key);
					var shrunk = Shrink.Input(state, key, value, obj => guard((T)obj));
					execution.SetShrinkOutcome(key, shrunk);
					return QAcidResult.Some(state, value);
				}

			default:
				{
					var value = generator.Generate();
					execution.SetIfNotAlreadyThere(key, value);
					return QAcidResult.Some(state, value);
				}
		}
	}
}
