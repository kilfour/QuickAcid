using System;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> OnceOnlyInput<T>(this string key, Func<T> func)
		{
			return
				s =>
					{
						var value = s.TempMemory.Get((object) key, func());
						if (s.Reporting)
						{
							//Console.WriteLine("{0} : {1}", key.ToString(), value.ToString());
							return new QAcidResult<T>(s, value);
						}
						return new QAcidResult<T>(s, value);
					};
		}
	}
}
