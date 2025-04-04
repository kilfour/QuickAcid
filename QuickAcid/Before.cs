namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<TOutput> Before<TOutput>(this QAcidRunner<TOutput> runner, Action act)
		{
			return state => { act(); return runner(state); };
		}
	}
}