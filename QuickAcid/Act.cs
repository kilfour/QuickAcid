using System;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
		{
			return
				state =>
					{
						try
						{
							var result = new QAcidResult<TOutput>(state, func());
						    state.LogReport(new QAcidReportActEntry(key));
						    return result;
						}
						catch (Exception exception)
						{
						    state.FailedWithException(exception);
						    state.LogReport(new QAcidReportActEntry(key, exception));
                            return new QAcidResult<TOutput>(state, default(TOutput));
						}
					};
		}

		public static QAcidRunner<Unit> Act(this string key, Action action)
		{
			return
			    state =>
				{
					try
					{
						action();
					    state.LogReport(new QAcidReportActEntry(key));
                        return new QAcidResult<Unit>(state, Unit.Instance);
					}
					catch (Exception exception)
					{
					    state.FailedWithException(exception);
					    state.LogReport(new QAcidReportActEntry(key, exception));
                        return new QAcidResult<Unit>(state, Unit.Instance);
					}
				};
		}
	}
}