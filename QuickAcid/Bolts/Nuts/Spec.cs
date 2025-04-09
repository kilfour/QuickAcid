using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts
{
	public static partial class QAcid
	{
		public static QAcidRunner<Acid> Spec(this string key, Func<bool> func)
		{
			return
				state =>
				{
					state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.SpecRunner });
					if (state.FailingSpec != null && state.FailingSpec != key) // PHASERS ON STUN
					{
						return new QAcidResult<Acid>(state, Acid.Test);
					}

					if (state.Verifying && state.FailingSpec == null) // PHASERS ON STUN
					{
						return new QAcidResult<Acid>(state, Acid.Test);
					}

					var result = func();
					if (!result)
					{
						state.SpecFailed(key);
					}
					return new QAcidResult<Acid>(state, Acid.Test);
				};
		}
	}
}