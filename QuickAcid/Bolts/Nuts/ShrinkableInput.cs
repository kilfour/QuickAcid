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
				if (state.ShrinkingInputs && !state.Shrunk.ForThisAction().ContainsKey(key))
				{
					var value = state.Memory.ForThisAction().Get<T>(key);
					var shrunk = Shrink.Input(state, key, value, obj => shrinkingGuard((T)obj));
					if (shrunk as string == "Irrelevant")
						state.Memory.ForThisAction().MarkAsIrrelevant<T>(key);
					else
						state.Memory.ForThisAction().AddReportingMessage<T>(key, shrunk.ToString()!);
					var valueAfterShrinking = state.Memory.ForThisAction().Get<T>(key);
					return QAcidResult.Some(state, valueAfterShrinking);
				}

				if (state.Verifying) // PHASERS ON STUN
				{
					return QAcidResult.Some(state, state.Memory.ForThisAction().Get<T>(key));
				}

				var value2 = generator.Generate();
				state.Memory.ForThisAction().SetIfNotAllReadyThere(key, value2);
				//state.Memory.ForThisAction().Set(key, value2);
				return QAcidResult.Some(state, value2);
			};
	}
}
