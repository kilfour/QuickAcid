namespace QuickAcid.Bolts.Nuts;

public static partial class QAcidCombinators
{
	public static QAcidRunner<TOutput> Before<TOutput>(this QAcidRunner<TOutput> runner, Action act)
	{
		return state =>
		{
			act();
			return runner(state);
		};
	}
}