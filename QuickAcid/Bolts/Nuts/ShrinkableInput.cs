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
				// PHASERS ON STUN
				if (state.ShrinkingInputs && !state.ShrinkableInputsTracker.ForThisExecution().AlreadyTried(key))
				{

					var value = state.Memory.ForThisExecution().Get<T>(key);
					var shrunk = Shrink.Input(state, key, value, obj => shrinkingGuard((T)obj));

					switch (shrunk)
					{
						case ShrinkOutcome.IrrelevantOutcome:
							state.Memory.ForThisExecution().MarkAsIrrelevant<T>(key);
							break;

						case ShrinkOutcome.ReportedOutcome(var msg):
							state.Memory.ForThisExecution().AddReportingMessage<T>(key, msg);
							break;
					}

					state.ShrinkableInputsTracker.ForThisExecution().MarkAsTriedToShrink(key);
					return QAcidResult.Some(state, value);
				}

				if (state.Verifying) // PHASERS ON STUN
				{
					return QAcidResult.Some(state, state.Memory.ForThisExecution().Get<T>(key));
				}

				var value2 = generator.Generate();
				state.Memory.ForThisExecution().SetIfNotAllReadyThere(key, value2);
				return QAcidResult.Some(state, value2);
			};
	}
}
