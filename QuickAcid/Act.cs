namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
		{
			return
				state =>
					{
						state.Memory.ForThisAction().ActionKey = key;
						try
						{
							var result = new QAcidResult<TOutput>(state, func());
							state.LogReport(new QAcidReportActEntry(key));
							return result;
						}
						catch (Exception exception)
						{
							state.Memory.ForThisAction().LastException = exception;
							state.FailedWithException(exception);
							state.LogReport(new QAcidReportActEntry(key, exception));
							return new QAcidResult<TOutput>(state, default(TOutput));
						}
					};
		}

		public static QAcidRunner<Acid> Act(this string key, Action action)
		{
			return
				state =>
				{
					state.Memory.ForThisAction().ActionKey = key;
					try
					{
						action();
						state.LogReport(new QAcidReportActEntry(key));
						return new QAcidResult<Acid>(state, Acid.Test);
					}
					catch (Exception exception)
					{
						state.Memory.ForThisAction().LastException = exception;
						state.FailedWithException(exception);
						state.LogReport(new QAcidReportActEntry(key, exception));
						return new QAcidResult<Acid>(state, Acid.Test);
					}
				};
		}
	}
}