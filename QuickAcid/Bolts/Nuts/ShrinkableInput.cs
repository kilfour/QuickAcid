using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> ShrinkableInput<T>(this string key, Generator<T> generator)
	{
		return ShrinkableInput<T>(key, generator, _ => true);
	}

	public static QAcidRunner<T> ShrinkableInput<T>(this string key, Generator<T> generator, Func<T, bool> shrinkingGuard)
	{
		return state =>
			{
				// PHASERS ON STUN
				if (state.ShrinkingInputs && !state.ShrinkableInputsTracker.ForThisExecution().AlreadyTried(key))
				{

					var value = state.Memory.ForThisAction().Get<T>(key);
					var shrunk = Shrink.Input(state, key, value, obj => shrinkingGuard((T)obj));

					switch (shrunk)
					{
						case ShrinkOutcome.IrrelevantOutcome:
							state.Memory.ForThisAction().MarkAsIrrelevant<T>(key);
							break;

						case ShrinkOutcome.ReportedOutcome(var msg):
							state.Memory.ForThisAction().AddReportingMessage<T>(key, msg);
							break;
					}

					state.ShrinkableInputsTracker.ForThisExecution().MarkAsTriedToShrink(key);
					return QAcidResult.Some(state, value);
				}

				if (state.Verifying) // PHASERS ON STUN
				{
					return QAcidResult.Some(state, state.Memory.ForThisAction().Get<T>(key));
				}

				var value2 = generator.Generate();
				state.Memory.ForThisAction().SetIfNotAllReadyThere(key, value2);
				return QAcidResult.Some(state, value2);
			};
	}
}
