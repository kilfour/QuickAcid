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
						state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.ActionRunner });
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
							return new QAcidResult<TOutput>(state, default(TOutput));
						}
					};
		}

		public static QAcidRunner<TOutput> ActIf<TOutput>(this string key, Func<bool> predicate, Func<TOutput> func)
		{
			return
				state =>
				{
					if (!predicate())
						return new QAcidResult<TOutput>(state, default(TOutput));
					return key.Act(func)(state);
				};
		}

		public static QAcidRunner<Acid> Act(this string key, Action action)
		{
			return
				state =>
				{
					state.MarkMyLocation(new Tracker { Key = key, RunnerType = RunnerType.ActionRunner });
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

		public static QAcidRunner<Acid> ActIf(this string key, Func<bool> predicate, Action func)
		{
			return
				state =>
				{
					if (!predicate())
						return new QAcidResult<Acid>(state, Acid.Test);
					return key.Act(func)(state);
				};
		}
	}
}