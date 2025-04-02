namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> OnceOnlyInput<T>(this string key, Func<T> func)
		{
			return
				s =>
					{
						var value = s.TempMemory.Get(key, func);
						if (s.Reporting)
						{
							return new QAcidResult<T>(s, value) { Key = key };
						}
						return new QAcidResult<T>(s, value) { Key = key };
					};
		}
	}
}
