using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<Acid> Spec(this string key, Func<bool> func)
	{
		return
			state =>
			{
				state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.SpecRunner });

				// PHASERS ON STUN
				if (state.FailingSpec != null && state.FailingSpec != key) // PHASERS ON STUN
				{
					return QAcidResult.AcidOnly(state);
				}

				// .Exception used to be State.Verifying, can be .ShrinkingExecutions ...
				// see if above, __Please_ put this thing out of it's misery
				if (state.Exception != null && state.FailingSpec == null) // PHASERS ON STUN
				{
					return QAcidResult.AcidOnly(state);
				}

				var result = func();
				if (!result)
				{
					state.SpecFailed(key);
				}
				return QAcidResult.AcidOnly(state);
			};
	}

	public static QAcidRunner<Acid> SpecIf(this string key, Func<bool> predicate, Func<bool> func)
	{
		return
			state =>
			{
				if (!predicate())
					return QAcidResult.AcidOnly(state);
				return key.Spec(func)(state);
			};
	}
}