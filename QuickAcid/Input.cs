using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> ToInput<T>(this string key, Generator<T> generator)
		{
			return
				s =>
				{
					if (s.Reporting)
					{
						var value1 = s.Memory.Get<T>(key);
						s.LogReport(string.Format("'{0}' : {1}.", key.ToString(), value1.ToString()));
						return new QAcidResult<T>(s, value1);
					}
					if (s.Shrinking || s.Verifying)
					{
						var value1 = s.Memory.Get<T>(key);
						return new QAcidResult<T>(s, value1);
					}
					var value2 = generator.Generate();
					s.Memory.Set(key, value2);
					return new QAcidResult<T>(s, value2);
				};
		}
		public static QAcidRunner<T> ToInput<T>(this Generator<T> generator, object key)
		{
			return
				s =>
				{
					if (s.Reporting)
					{
						var value1 = s.Memory.Get<T>(key);
						s.LogReport(string.Format("'{0}' : {1}.", key.ToString(), value1.ToString()));
						return new QAcidResult<T>(s, value1);
					}
					if (s.Shrinking || s.Verifying)
					{
						var value1 = s.Memory.Get<T>(key);
						return new QAcidResult<T>(s, value1);
					}
					var value2 = generator.Generate();
					s.Memory.Set(key, value2);
					return new QAcidResult<T>(s, value2);
				};
		}
	}
}
