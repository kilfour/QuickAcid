using System;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> Input<T>(this string key, Generator<T> generator)
		{
			return Input(key, generator.Generate);
		}

		public static QAcidRunner<T> Input<T>(this string key, Func<T> func, Func<T, string> stringify = null)
		{
			return
				s =>
				{
					if (s.Reporting)
					{
						var value1 = s.Memory.Get<T>(key);
					    var entry =
					        new QAcidReportInputEntry(key)
					        {
					            Value = stringify?.Invoke(value1)
                            };
					    s.LogReport(entry);
						return new QAcidResult<T>(s, value1) { Key = key };
					}
					if (s.Shrinking || s.Verifying)
					{
						var value1 = s.Memory.Get<T>(key);
						return new QAcidResult<T>(s, value1) { Key = key };
					}
					var value2 = func();
					s.Memory.Set(key, value2);
					return new QAcidResult<T>(s, value2) { Key = key };
				};
		}
	}
}
