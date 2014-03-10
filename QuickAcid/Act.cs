using System;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<TOutput> Act<TOutput>(this string key, Func<TOutput> func)
		{
			return
				s =>
					{
						if (s.Reporting)
						{
							s.LogReport(string.Format("Executed : '{0}'.", key.ToString()));
							//return new QAcidResult<TOutput>(s, default(TOutput));
						}
						try
						{
							return new QAcidResult<TOutput>(s, func());
						}
						catch (Exception exception)
						{
							s.FailedWithException(exception);
							return new QAcidResult<TOutput>(s, default(TOutput));
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
						s.LogReport(string.Format("Executed : '{0}'.", key.ToString()));
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