using System;
using QuickMGenerate;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> If<T>(this string key, Func<bool> predicate, QAcidRunner<T> runner)
		{
			return
				s =>
				{
					if(predicate())
						return runner(s);
					return new QAcidResult<T>(s, default(T));
				};
		}
	}
}