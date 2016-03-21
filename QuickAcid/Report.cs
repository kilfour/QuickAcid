using System;
using QuickMGenerate;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> Report<T>(this QAcidRunner<T> runner, Func<T, string> toString)
		{
			return
				s =>
				{
					if (s.Reporting)
					{
						s.DontReport = true;
						var result = runner(s);
						s.DontReport = false;
						s.LogReport(string.Format("'{0}' : {1}.", result.Key, toString(result.Value)));
						return result;
					}
					return runner(s);
				};
		}

		public static QAcidRunner<T> DontReport<T>(this QAcidRunner<T> runner)
		{
			return
				s =>
				{
					if (s.Reporting)
					{
						s.DontReport = true;
						var result = runner(s);
						s.DontReport = false;
						return result;
					}
					return runner(s);
				};
		}
	}
}