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
						if (state.Reporting)
						{
						    state.LogReport(new AcidReportActEntry(key));
                            return new QAcidResult<TOutput>(state, default(TOutput));
						}
						try
						{
							return new QAcidResult<TOutput>(state, func());
						}
						catch (Exception exception)
						{
						    state.FailedWithException(exception);
							return new QAcidResult<TOutput>(state, default(TOutput));
						}
					};
		}

		public static QAcidRunner<Unit> Act(this string key, Action action)
		{
			return
				s =>
				{
					if (s.Reporting)
					{
					    s.LogReport(new AcidReportActEntry(key));
                        return new QAcidResult<Unit>(s, Unit.Instance);
					}
					try
					{
						action();
						return new QAcidResult<Unit>(s, Unit.Instance);
					}
					catch (Exception exception)
					{
						s.FailedWithException(exception);
						return new QAcidResult<Unit>(s, Unit.Instance);
					}
				};
		}
	}
}