using QuickAcid.CodeGen;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<Acid> Spec(this string key, Func<bool> func)
		{
			return
				state =>
				{
					state.XMarksTheSpot.TheTracker =
							new Tracker { Key = key, RunnerType = RunnerType.SpecRunner };
					if (state.FailingSpec != null && state.FailingSpec != key)
					{
						return new QAcidResult<Acid>(state, Acid.Test);
					}

					if (state.Verifying && state.FailingSpec == null)
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