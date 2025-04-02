namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> OnceOnlyInput<T>(this string key, Func<T> func)
		{
			return
				s =>
					{
						var value = s.Memory.OnceOnlyInputsPerRun.GetOrAdd(key, func);
						return new QAcidResult<T>(s, value) { Key = key };
					};
		}
	}
}
