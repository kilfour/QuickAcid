using QuickAcid.CodeGen;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
		{
			return
				state =>
					{
						state.XMarksTheSpot.TheTracker =
							new Tracker { Key = key, RunnerType = RunnerType.ActionRunner };
						state.Memory.ForThisAction().ActionKey = key;
						try
						{
							var result = new QAcidResult<TOutput>(state, func());
							return result;
						}
						catch (Exception exception)
						{
							state.Memory.ForThisAction().LastException = exception;
							state.FailedWithException(exception);
							return new QAcidResult<TOutput>(state, default);
						}
					};
		}

		public static QAcidRunner<Acid> Act(this string key, Action action)
		{
			return
				state =>
				{
					state.XMarksTheSpot.TheTracker =
							new Tracker { Key = key, RunnerType = RunnerType.ActionRunner };
					state.Memory.ForThisAction().ActionKey = key;
					try
					{
						action();
						return new QAcidResult<Acid>(state, Acid.Test);
					}
					catch (Exception exception)
					{
						state.Memory.ForThisAction().LastException = exception;
						state.FailedWithException(exception);
						return new QAcidResult<Acid>(state, Acid.Test);
					}
				};
		}
	}
}