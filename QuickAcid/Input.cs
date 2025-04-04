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

		public static QAcidRunner<T> InputIf<T>(this string key, Func<bool> predicate, Generator<T> generator)
		{
			if (!predicate())
				return s => new QAcidResult<T>(s, default!);
			return Input(key, generator.Generate);
		}

		public static QAcidRunner<T> Input<T>(this string key, Func<T> func, Func<T, string> stringify = null)
		{
			return
				s =>
				{
					if (s.Shrinking || s.Verifying)
					{
						var value1 = s.Memory.ForThisAction().Get<T>(key);
						return new QAcidResult<T>(s, value1) { Key = key };
					}
					var value2 = func();
					s.Memory.ForThisAction().Set(key, value2);
					return new QAcidResult<T>(s, value2) { Key = key };
				};
		}

		public static QAcidRunner<T> InputIf<T>(this string key, Func<bool> predicate, Func<T> func, Func<T, string> stringify = null)
		{
			if (!predicate())
				return s => new QAcidResult<T>(s, default!);

			return Input(key, func, stringify);
		}
	}
}
