using QuickMGenerate;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> Choose<T>(this string key, params QAcidRunner<T>[] runners)
		{
			return
				s =>
				{
					int value;
					if (s.IsNormalRun())
					{
						value = MGen.Int(0, runners.Length).Generate();
						s.Memory.Set(key, value);
					}
					else
						value = s.Memory.Get<int>(key);
					return runners[value](s);
				};
		}
    }
}